﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosReadCardDialog : PosBaseDialog
    {
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        private readonly EntryBoxValidation _entryBoxMovementDescription;

        public string CardNumber { get; set; }

        public PosReadCardDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Settings
            string regexAlfaNumericExtended = RegexUtils.RegexAlfaNumericExtended;

            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_readcard");
            Size windowSize = new Size(462, 320);//400 With Other Payments
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_read_card.png";

            //EntryDescription
            _entryBoxMovementDescription = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_read_card"), KeyboardMode.AlfaNumeric, regexAlfaNumericExtended, false);
            //_entryBoxMovementDescription.EntryValidation.Changed += delegate { ValidateDialog(); };
            //VBox
            VBox vbox = new VBox(true, 0);
            //vbox.PackStart(_entryBoxMovementAmountOtherPayments, true, true, 0);
            vbox.PackStart(_entryBoxMovementDescription, true, true, 0);

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(vbox, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            _buttonOk.Clicked += _buttonOk_Clicked;

            this.KeyReleaseEvent += PosReadCardDialog_KeyReleaseEvent;
            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        private void PosReadCardDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                CardNumber = _entryBoxMovementDescription.EntryValidation.Text;
                _buttonOk.Click();
            }
        }

        private void _buttonOk_Clicked(object sender, EventArgs e)
        {
            CardNumber = _entryBoxMovementDescription.EntryValidation.Text;
        }
    }
}
