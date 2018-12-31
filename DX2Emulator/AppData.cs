using DX2Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX2Emulator
{
    public class AppData
    {
        static AppData inst_;

        public ObservableCollection<Team> Teams { get; private set; } = new ObservableCollection<Team>();
        public DemonDB DemonDB { get; private set; }
        public List<Skill> Skills { get; private set; }

        static AppData()
        {
            inst_ = new AppData();
        }

        AppData()
        {
            Skills = Skill.ReadSkills(AppUtil.GetLocalFilePath("json/Skills.json"));
            Skills.Sort((l,r) => { return l.Name.CompareTo(r.Name); });
            DemonDB = new DemonDB();
            DemonDB.Load(AppUtil.GetLocalFilePath("json/Demons.json"), Skills);
        }

        public static AppData GetInst() { return inst_; }
    }
}
