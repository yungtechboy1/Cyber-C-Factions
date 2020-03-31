using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.ClassFactory.Window
{
    public class MainClassWindowLearnedPowers : CyberFormSimple
    {
        [JsonIgnore]
        BaseClass _BC ;
        
        public MainClassWindowLearnedPowers(BaseClass bc) : base(MainForm.Main_Class_Settings_Window_Learned_Power,"You're "+bc.getDisplayName()+" Class Learned Powers")
        {
            _BC = bc;
            foreach (var ape in bc.getClassSettings().getLearnedPowers())
            {
                addButton(new Button(){Text = ape.getPowerEnum()+" Power "+ ape.getValue(), ExecuteAction = onRun});
            }
        }

        public void onRun(Player player, SimpleForm simpleForm)
        {
            CorePlayer p = (CorePlayer) player;
            player.SendForm(p.getPlayerClass().getSettingsWindow());
        }
    }
}