﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Extensions;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    //KeyboardKey UI Key Widgets
    public class KeyboardPadKey : TouchButtonBase
    {
        //Private Members
        private readonly string _l1LabelText;
        private readonly string _l2LabelText;
        private readonly Color _colorKeyboardPadKeyDefaultFont = LogicPOS.Settings.GeneralSettings.Settings["colorKeyboardPadKeyDefaultFont"].StringToColor();
        private readonly Color _colorKeyboardPadKeySecondaryFont = LogicPOS.Settings.GeneralSettings.Settings["colorKeyboardPadKeySecondaryFont"].StringToColor();
        private readonly Color _colorKeyboardPadKeyBackground = LogicPOS.Settings.GeneralSettings.Settings["colorKeyboardPadKeyBackground"].StringToColor();
        private readonly Color _colorKeyboardPadKeyBackgroundActive = LogicPOS.Settings.GeneralSettings.Settings["colorKeyboardPadKeyBackgroundActive"].StringToColor();

        public Label LabelL1 { get; set; }
        public Label LabelL2 { get; set; }
        //Store VirtualKey in Properties
        public VirtualKey Properties { get; set; }
        //Active Modifier Key, Change Color
        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                if (_active)
                {
                    SetBackgroundColor(_colorKeyboardPadKeyBackgroundActive, _eventboxBackgroundColor);
                }
                else
                {
                    SetBackgroundColor(_colorKeyboardPadKeyBackground, _eventboxBackgroundColor);
                }
            }
        }

        //Constructor
        public KeyboardPadKey(VirtualKey virtualKey)
            : base("")
        {
            //Always Store full VirtualKey Properties in KeyBoardKey
            this.Properties = virtualKey;

            //Key Labels
            if (virtualKey.L1 != null) { _l1LabelText = virtualKey.L1.Glyph; } else { _l1LabelText = ""; };
            if (virtualKey.L2 != null) { _l2LabelText = virtualKey.L2.Glyph; } else { _l2LabelText = ""; };

            //Init Local Vars
            Size sizeKeyboardPadDefaultKey = logicpos.Utils.StringToSize(LogicPOS.Settings.GeneralSettings.Settings["sizeKeyboardPadDefaultKey"]);
            string fontKeyboardPadPrimaryKey = LogicPOS.Settings.GeneralSettings.Settings["fontKeyboardPadPrimaryKey"];
            string fontKeyboardPadSecondaryKey = LogicPOS.Settings.GeneralSettings.Settings["fontKeyboardPadSecondaryKey"];

            //ByPass Defaults
            if (virtualKey.L1.KeyWidth > 0)
            {
                sizeKeyboardPadDefaultKey.Width = virtualKey.L1.KeyWidth;
            };

            //Defaults
            //Prepare Pango Fonts      
            if (virtualKey.L1.IsBold) { fontKeyboardPadPrimaryKey = "Bold " + fontKeyboardPadPrimaryKey; };
            Pango.FontDescription fontDescriptionPrimaryKey = Pango.FontDescription.FromString(fontKeyboardPadPrimaryKey);
            Pango.FontDescription fontDescriptionSecondaryKey = Pango.FontDescription.FromString(fontKeyboardPadSecondaryKey);

            //Vbox
            VBox vbox = new VBox(true, 0);
            vbox.BorderWidth = 2;
            //Labels
            LabelL1 = new Label();
            LabelL1.Text = _l1LabelText;
            //Align
            switch (virtualKey.L1.HAlign)
            {
                case "left":
                    LabelL1.SetAlignment(0.00F, 0.5F);
                    break;
                case "right":
                    LabelL1.SetAlignment(1.00F, 0.5F);
                    break;
            }
            LabelL1.ModifyFg(StateType.Normal, _colorKeyboardPadKeyDefaultFont.ToGdkColor());
            LabelL1.ModifyFont(fontDescriptionPrimaryKey);
            vbox.PackEnd(LabelL1);
            //HideL2 dont show L2
            if (_l2LabelText != string.Empty && !virtualKey.L1.HideL2)
            {
                LabelL2 = new Label();
                LabelL2.Text = _l2LabelText;
                LabelL1.ModifyFg(StateType.Normal, _colorKeyboardPadKeyDefaultFont.ToGdkColor());
                LabelL2.ModifyFg(StateType.Normal, _colorKeyboardPadKeySecondaryFont.ToGdkColor());
                LabelL1.ModifyFont(fontDescriptionSecondaryKey);
                LabelL2.ModifyFont(fontDescriptionSecondaryKey);
                LabelL1.SetAlignment(0.60F, 0.50F);
                LabelL2.SetAlignment(0.40F, 0.50F);
                vbox.PackStart(LabelL2);
            };

            //InitObject("", _colorKeyboardPadKeyBackground, vbox, sizeKeyboardPadDefaultKey.Width, sizeKeyboardPadDefaultKey.Height);

            //Changed for theme
            this.BorderWidth = 1;
            InitObject("", Color.Transparent, vbox, sizeKeyboardPadDefaultKey.Width, sizeKeyboardPadDefaultKey.Height);
        }
    }
}
