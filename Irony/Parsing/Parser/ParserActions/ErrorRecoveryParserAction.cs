using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    //TODO: Improve recovery by adding automatic injection of missing tokens.
    // Make sure we ALWAYS have output parse tree, even if it is messed up
    public class ErrorRecoveryParserAction : ParserAction
    {

        public override void Execute(ParsingContext context)
        {
            context.Status = ParserStatus.Error;
            var grammar = context.Language.Grammar;
            grammar.ReportParseError(context);
            // Do not recover if we're already at EOF, or if we're in command line mode
            //if (context.CurrentParserInput.Term == grammar.Eof || context.Mode == ParseMode.CommandLine)
            //  return;
            if (context.Mode == ParseMode.CommandLine)
                return;

            //Try to recover from error
            context.Status = ParserStatus.Recovering;
           // context.AddTrace(Resources.MsgTraceRecovering); // *** RECOVERING - searching for state with error shift *** 
            
            var recovered = TryRecoverFromError(context);
            if (recovered)
            {
                //context.AddTrace(Resources.MsgTraceRecoverSuccess); //add new trace entry
                context.Status = ParserStatus.Parsing;
            }
            else
            {
                context.AddTrace(Resources.MsgTraceRecoverFailed);
                context.Status = ParserStatus.Error;
            }
        }

        protected bool TryRecoverFromError(ParsingContext context)
        {
            var grammar = context.Language.Grammar;
            var parser = context.Parser;


            //// On va essayer de faire une réduction en cherchant si il y en a une dans les actions possibles
            //ParserAction reduceAction = context.CurrentParserState.Actions.Values.FirstOrDefault(s => s is ReduceParserAction);
            //if (reduceAction != null)
            //{
            //    reduceAction.Execute(context);

            //    TryToShiftExpectedTerminal(context);
            //    return true;
            //}

            // Sinon si il n'y a qu'une reduction possible on la simule
            var shiftActions = context.CurrentParserState.Actions.Values.Where(s => s is ShiftParserAction).ToList();
            if( shiftActions.Count == 1)
            {
                var term = context.CurrentParserState.ExpectedTerminals.First();
                context.CurrentParserInput = new ParseTreeNode(new Token(term, context.Source.Location, term.ToString(), term.ToString() ));
                shiftActions[0].Execute(context);
                return true;
            }

            //Stack<Tuple<ParserState, int>> actions = new Stack<Tuple<ParserState, int>>();
            //var reduceAction = FindFirstReduceAction(actions, context.CurrentParserState);
            //if (reduceAction != null)
            //{
            //    var savedParserInput = context.CurrentParserInput;
            //    foreach (var tuple in actions.Reverse())
            //    {
            //        var state = tuple.Item1;
            //        var action = state.Actions.Skip(tuple.Item2).First().Value;

            //        //var term = state.ExpectedTerminals.Skip(tuple.Item2).FirstOrDefault();
            //        //if (term != null)
            //        //{
            //        //    context.CurrentParserInput = new ParseTreeNode(new Token(term, context.Source.Location, term.ToString(), term.ToString()));
            //        savedParserInput = context.CurrentParserInput;
            //        //}
            //        action.Execute(context);
            //    }

            //    context.CurrentParserInput = savedParserInput;
            //    reduceAction.Execute(context);

            //    TryToShiftExpectedTerminal(context);
            //    return true;
            //}

            context.HasFatalErrors = true;

            //1. We need to find a state in the stack that has a shift item based on error production (with error token), 
            // and error terminal is current. This state would have a shift action on error token. 
            ParserAction errorShiftAction = FindErrorShiftActionInStack(context);
            if (errorShiftAction == null) return false; //we failed to recover
            context.AddTrace(Resources.MsgTraceRecoverFoundState, context.CurrentParserState);
            //2. Shift error token - execute shift action
            context.AddTrace(Resources.MsgTraceRecoverShiftError, errorShiftAction);
            errorShiftAction.Execute(context);
            //4. Now we need to go along error production until the end, shifting tokens that CAN be shifted and ignoring others.
            //   We shift until we can reduce
            context.AddTrace(Resources.MsgTraceRecoverShiftTillEnd);
            while (true)
            {
                if (context.CurrentParserInput == null)
                    parser.ReadInput();
                if (context.CurrentParserInput.Term == grammar.Eof)
                    return false;
                //Check if we can reduce
                var nextAction = parser.GetNextAction();
                if (nextAction == null)
                {
                    parser.ReadInput();
                    continue;
                }
                if (nextAction is ReduceParserAction)
                {
                    //We are reducing a fragment containing error - this is the end of recovery
                    //Clear all input token queues and buffered input, reset location back to input position token queues; 
                    context.SetSourceLocation(context.CurrentParserInput.Span.Location);

                    //Reduce error production - it creates parent non-terminal that "hides" error inside
                    context.AddTrace(Resources.MsgTraceRecoverReducing);
                    context.AddTrace(Resources.MsgTraceRecoverAction, nextAction);
                    nextAction.Execute(context); //execute reduce
                    context.HasFatalErrors = false;
                    return true; //we recovered 
                }
                // If it is not reduce, simply execute it (it is most likely shift)
                context.AddTrace(Resources.MsgTraceRecoverAction, nextAction);
                nextAction.Execute(context); //shift input token
            }
        }//method

        //private static void TryToShiftExpectedTerminal(ParsingContext context)
        //{
        //    var term = context.CurrentParserState.ExpectedTerminals.FirstOrDefault();
        //    if (term != null)
        //    {
        //        var newToken = new Token(term, context.Source.Location, term.ToString(), term.ToString());
        //        if (context.CurrentParserInput != null)
        //        {
        //            var ln = context.CurrentParseTree.Tokens.Count;
        //            if (ln > 0)
        //                context.CurrentParseTree.Tokens[ln - 1] = newToken;
        //            context.BufferedTokens.Push(context.CurrentToken);
        //        }
        //        context.CurrentParserInput = new ParseTreeNode(newToken);
        //    }
        //}

        //private static ParserAction FindFirstReduceAction(Stack<Tuple<ParserState, int>> actions, ParserState state)
        //{
        //    var x = 0;
        //    foreach (var sa in state.Actions.Where(v=>v.Value is ShiftParserAction))
        //    {
        //        actions.Push(Tuple.Create(state, x++));

        //        var shiftAction = sa.Value as ShiftParserAction;
        //        var reduceAction = shiftAction.NewState.Actions.Values.FirstOrDefault(s => s is ReduceParserAction) ?? FindFirstReduceAction(actions, shiftAction.NewState);
        //        if (reduceAction != null)
        //        {
        //            return reduceAction;
        //        }
        //        actions.Pop();
        //    }
        //    return null;
        //}

        private ParserAction FindErrorShiftActionInStack(ParsingContext context)
        {
            var grammar = context.Language.Grammar;
            while (context.ParserStack.Count >= 1)
            {
                ParserAction errorShiftAction;
                if (context.CurrentParserState.Actions.TryGetValue(grammar.SyntaxError, out errorShiftAction)
                     && errorShiftAction is ShiftParserAction)
                    return errorShiftAction;
                //pop next state from stack
                if (context.ParserStack.Count == 1)
                    return null; //don't pop the initial state
                context.ParserStack.Pop();
                context.CurrentParserState = context.ParserStack.Top.State;
            }
            return null;
        }

    }//class
}//ns
