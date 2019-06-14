﻿using System;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace Vim.UI.Wpf.Implementation.Paste
{
    internal sealed class PasteController
    {
        private readonly IVimBuffer _vimBuffer;
        private readonly PasteAdornment _pasteAdornment;

        internal PasteController(
            IVimBuffer vimBuffer,
            IWpfTextView wpfTextView,
            IProtectedOperations protectedOperations,
            IClassificationFormatMap classificationFormatMap,
            IEditorFormatMap editorFormatMap)
        {
            _vimBuffer = vimBuffer;
            _pasteAdornment = new PasteAdornment(
                wpfTextView,
                wpfTextView.GetAdornmentLayer(PasteFactoryService.PasteAdornmentLayerName),
                protectedOperations,
                classificationFormatMap,
                editorFormatMap);

            _vimBuffer.KeyInputProcessed += OnKeyInputProcessed;
            _vimBuffer.Closed += OnVimBufferClosed;
        }

        private void OnKeyInputProcessed(object sender, EventArgs e)
        {
            if (_vimBuffer.ModeKind == ModeKind.Insert && _vimBuffer.InsertMode.IsInPaste)
            {
                _pasteAdornment.PasteCharacter = _vimBuffer.InsertMode.PasteCharacter.Value;
                _pasteAdornment.IsDisplayed = true;
            }
            else
            {
                _pasteAdornment.IsDisplayed = false;
            }
        }

        private void OnVimBufferClosed(object sender, EventArgs e)
        {
            _pasteAdornment.Destroy();
            _vimBuffer.KeyInputProcessed -= OnKeyInputProcessed;
            _vimBuffer.Closed -= OnVimBufferClosed;
        }
    }
}
