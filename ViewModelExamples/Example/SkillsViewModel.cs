using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.ObjectModel;

using MVVM.ViewModel.Infra;

namespace MVVM.ViewModel.Example
{
    public class SkillsViewModel : ViewModelBase
    {
        public SkillsViewModel()
        {
            RemoveSkill = new RelayCommand<Skill>(s => this.Skills.Remove(s));
            AddSkill = new RelayCommand<Skill>(s => { MainSkill = new Skill() {Type="Type",Name="New skill" }; this.Skills.Add(MainSkill); });
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

        public IList<Skill> Skills { get; private set; }

        public IList<Skill> SelectedSkills { get; private set; }

        public ICommand RemoveSkill { get; private set; }

        public ICommand AddSkill { get; private set; }
    }
}
