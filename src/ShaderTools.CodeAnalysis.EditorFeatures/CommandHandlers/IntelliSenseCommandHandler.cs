﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace ShaderTools.CodeAnalysis.Editor.CommandHandlers
{
    /// <summary>
    /// There are two forms of intellisense that may be active at the same time.  Completion and
    /// SigHelp.  Completion precedes SigHelp in our command handler because it wants to make sure
    /// it's operating on a buffer *after* Completion has changed it.  i.e. if "WriteL(" is typed,
    /// sig help wants to allow completion to complete that to "WriteLine(" before it tried to
    /// proffer sig help.  If we were to reverse things, then we'd get a bogus situation where sig
    /// help would see "WriteL(" would have nothing to offer and would return.
    /// 
    /// However, despite wanting sighelp to receive typechar first and then defer it to completion,
    /// we want completion to receive other events first (like escape, and navigation keys).  We
    /// consider completion to have higher priority for those commands.  In order to accomplish that,
    /// both of the sig help and completion command handlers are imported by this command handler.
    /// This command handler then delegates escape, up and down to those command handlers.  It always
    /// tries the completion command handler first, and defers to signature help if completion
    /// doesn't process the command.
    /// </summary>
    [Export(typeof(ICommandHandler))]
    [ContentType(ContentTypeNames.ShaderToolsContentType)]
    [Name(nameof(IntelliSenseCommandHandler))]
    internal sealed class IntelliSenseCommandHandler : 
        IChainedCommandHandler<EscapeKeyCommandArgs>,
        IChainedCommandHandler<UpKeyCommandArgs>,
        IChainedCommandHandler<DownKeyCommandArgs>
    {
        private readonly SignatureHelpCommandHandler _signatureHelpCommandHandler;

        public string DisplayName => "Signature Help";

        [ImportingConstructor]
        public IntelliSenseCommandHandler(SignatureHelpCommandHandler signatureHelpCommandHandler)
        {
            _signatureHelpCommandHandler = signatureHelpCommandHandler;
        }

        public CommandState GetCommandState(EscapeKeyCommandArgs args, Func<CommandState> nextHandler)
        {
            return nextHandler();
        }

        public CommandState GetCommandState(UpKeyCommandArgs args, Func<CommandState> nextHandler)
        {
            return nextHandler();
        }

        public CommandState GetCommandState(DownKeyCommandArgs args, Func<CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(EscapeKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if ((_signatureHelpCommandHandler != null && _signatureHelpCommandHandler.TryHandleEscapeKey(args)))
            {
                return;
            }

            nextHandler();
        }

        public void ExecuteCommand(UpKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if ((_signatureHelpCommandHandler != null && _signatureHelpCommandHandler.TryHandleUpKey(args)))
            {
                return;
            }

            nextHandler();
        }

        public void ExecuteCommand(DownKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if ((_signatureHelpCommandHandler != null && _signatureHelpCommandHandler.TryHandleDownKey(args)))
            {
                return;
            }

            nextHandler();
        }
    }
}
