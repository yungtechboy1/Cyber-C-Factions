using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.Forms;
using MiNET.Blocks;
using MiNET.Utils;
using Newtonsoft.Json;
using Button = MiNET.UI.Button;

namespace CyberCore.Manager.ClassFactory.Window
{
    public class MainClassSettingsWindow : CyberFormSimple

    {
        [JsonIgnore] BaseClass _BC;

        public MainClassSettingsWindow(BaseClass bd, MainForm ttype, String title, String content) : base(ttype,
            new List<MiNET.UI.Button>(), title + "\n" + content)
        {
            _BC = bd;
            inti();
        }

        public MainClassSettingsWindow(BaseClass bd, MainForm ttype, String title, String content,
            List<Button> buttons) : base(ttype, buttons, title + "\n" + content)

        {
            _BC = bd;
            inti();
        }

        private void inti()
        {
            addButton(new Button()
            {
                Text = "How to use " + ChatColors.Aqua + _BC.getName(), ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(b.getPlayerClass().getHowToUseClassWindow());
                    }
            });
            addButton(new Button()
            {
                Text = "Class Merchant", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(b.getPlayerClass().getClassMerchantWindow());
                    }
            });
            addButton(new Button()
            {
                Text = ChatColors.Red + "Leave Class", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        CyberCoreMain.GetInstance().ClassFactory.leaveClass(b);
                    }
            });
            addButton(new Button()
            {
                Text = ChatColors.Yellow + "----Powers---", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(b.getPlayerClass().getHowToUseClassWindow());
                    }
            });
            addButton(new Button()
            {
                Text = "Choose Active Powers", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(new MainClassSettingsWindowActivePowers(b.getPlayerClass()));
                    }
            });
            addButton(new Button()
            {
                Text = "Set Preferred Power For Slot 9", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(new MainClassSettingsWindowChooseLockedSlot(b.getPlayerClass(), LockedSlot.SLOT_9));
                    }
            });
            addButton(new Button()
            {
                Text = "Set Preferred Power For Slot 8", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(new MainClassSettingsWindowChooseLockedSlot(b.getPlayerClass(), LockedSlot.SLOT_8));
                    }
            });
            addButton(new Button()
            {
                Text = "Set Preferred Power For Slot 7", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        b?.SendForm(new MainClassSettingsWindowChooseLockedSlot(b.getPlayerClass(), LockedSlot.SLOT_7));
                    }
            });
            addButton(new Button()
            {
                Text = ChatColors.Black + "-------------", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        // b?.SendForm(b.getPlayerClass().getHowToUseClassWindow());
                    }
            });
            addButton(new Button() {Text = "Learned Powers", ExecuteAction =
                (p, form) =>
                {
                    var b = (CorePlayer) p;
                    b?.SendForm(new MainClassWindowLearnedPowers(b.getPlayerClass()));
                }
            });
            //Plugin Callback
            _BC.addButtons(this);
            addButton(new Button()
            {
                Text = ChatColors.Black + "-------------", ExecuteAction =
                    (p, form) =>
                    {
                        var b = (CorePlayer) p;
                        // b?.SendForm(b.getPlayerClass().getHowToUseClassWindow());
                    }
            });
        }

    //     /**
    //  * Return True only if a Response has been executed
    //  *
    //  * @param p CorePlayer
    //  * @return bool
    //  */
    //     public bool onRun(CorePlayer p)
    //     {
    //         int k = getResponse().getClickedButtonId();
    //         if (k == getKey())
    //         {
    //             //TODO
    //             p.showFormWindow(p.getPlayerClass().getHowToUseClassWindow());
    //         }
    //         else if (k == getKey())
    //         {
    //             p.showFormWindow(p.getPlayerClass().getClassMerchantWindow());
    //         }
    //         else if (k == getKey())
    //         {
    //             CyberCoreMain.getInstance().ClassFactory.leaveClass(p);
    //         }
    //         else if (k == getKey())
    //         {
    //             //Null
    //         }
    //         else if (k == getKey())
    //         {
    //             //Choose active Powers
    //             p.showFormWindow(new MainClassSettingsWindowActivePowers(p.getPlayerClass()));
    //         }
    //         else if (k == getKey())
    //         {
    //             //Choose active Powers
    //             p.showFormWindow(new MainClassSettingsWindowChooseLockedSlot(p.getPlayerClass(), LockedSlot.SLOT_9));
    //         }
    //         else if (k == getKey())
    //         {
    //             //Choose active Powers
    //             p.showFormWindow(new MainClassSettingsWindowChooseLockedSlot(p.getPlayerClass(), LockedSlot.SLOT_8));
    //         }
    //         else if (k == getKey())
    //         {
    //             //Choose active Powers
    //             p.showFormWindow(new MainClassSettingsWindowChooseLockedSlot(p.getPlayerClass(), LockedSlot.SLOT_7));
    //         }
    //         else if (k == getKey())
    //         {
    //             //Null
    //         }
    //         else if (k == getKey())
    //         {
    //             //Choose active Powers
    //             p.showFormWindow(new MainClassWindowLearnedPowers(p.getPlayerClass()));
    //         }
    //
    //         return true;
    //     }
    // }
}
}
