#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Irony.Parsing;

namespace Irony {

  public enum ErrorLevel {
    Info = 0,
    Warning = 1,
    Error = 2,
  }

  //Container for syntax errors and warnings
  public class LogMessage {
      public LogMessage(ErrorLevel level, SourceSpan location, string message, ParserState parserState)
      {
      Level = level;
      SourceSpan = location;
      Message = message;
      ParserState = parserState;
    }

    public readonly ErrorLevel Level;
    public readonly ParserState ParserState;
    public readonly SourceSpan SourceSpan;
    public readonly string Message;

    public override string ToString() {
      return Message;
    }
  }//class

  public class LogMessageList : List<LogMessage> {
    public static int ByLocation(LogMessage x, LogMessage y) {
        return SourceLocation.Compare(x.SourceSpan.Location, y.SourceSpan.Location);
    }
  }

}//namespace
