using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel {
    public class SkillsViewModel : ViewModelBase
    {
        public SkillsViewModel()
        {
            RemoveSkill = new RelaySimpleCommand<Skill>(s => this.Skills.Remove(s));
            AddSkill = new RelaySimpleCommand(() => { MainSkill = new Skill() {Type="Type",Name="New skill" }; this.Skills.Add(MainSkill); });
            Skills = new ObservableCollection<Skill>();
            SelectedSkills = new ObservableCollection<Skill>();
        }


        private Skill _MainSkill;
        public Skill MainSkill
        {
            get { return _MainSkill; }
            set
            {
                Set(ref _MainSkill, value, "MainSkill");
            }
        }

        public IList<Skill> Skills { get; }

        public IList<Skill> SelectedSkills { get;}

        public ICommand RemoveSkill { get; }

        public ICommand AddSkill { get; }
    }
}
