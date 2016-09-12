using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MVVM.ViewModel.Infra;
using Neutronium.Core.Infra;
using Neutronium.MVVMComponents;

namespace MVVM.ViewModel.Example
{
    public class Person : ViewModelBase
    {
        public Person(ICommand forTest=null)
        {
            Skills = new ObservableCollection<Skill>();

            TestCommand = forTest;
            Command = new ToogleRelayCommand(DoCommand);
            RemoveSkill = new RelayCommand<Skill>(s=> this.Skills.Remove(s));
            ChangeSkill = new RelayCommand<Skill>(s => MainSkill = (this.Skills.Count>0)?this.Skills[0] : null);
            RemoveSkills = new RelayCommand<Skill>(s => Skills.Clear());
        }

        private void DoCommand()
        {
            Local = new Local() { City = "Paris", Region = "IDF" };
            Skills.Insert(0, new Skill() { Name = "Info", Type = "javascript" });
            Command.ShouldExecute = false;
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { Set(ref _LastName, value, "LastName"); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }

        private DateTime _BirthDay;
        public DateTime BirthDay
        {
            get { return _BirthDay; }
            set { Set(ref _BirthDay, value, "BirthDay"); }
        }

        private PersonalState _PersonalState;
        public PersonalState PersonalState
        {
            get { return _PersonalState; }
            set { Set(ref _PersonalState, value, "PersonalState"); }
        }

        private Sex _Sex;
        public Sex Sex
        {
            get { return _Sex; }
            set { Set(ref _Sex, value, "Sex"); }
        }

        private int _Age;
        public int Age
        {
            get { return _Age; }
            set { Set(ref _Age, value, "Age"); }
        }

        private int? _ChildrenNumber;
        public int? ChildrenNumber
        {
            get { return _ChildrenNumber; }
            set { Set(ref _ChildrenNumber, value, "ChildrenNumber"); }
        }

        private Local _Local;
        public Local Local
        {
            get { return _Local; }
            set { Set(ref _Local, value, "Local"); }
        }

        private Skill _MainSkill;
        public Skill MainSkill
        {
            get { return _MainSkill; }
            set { Set(ref _MainSkill, value, "MainSkill"); }
        }

        public IEnumerable<PersonalState> States => EnumExtender.GetEnums<PersonalState>();

        public IEnumerable<Sex> Sexes => EnumExtender.GetEnums<Sex>();

        public IList<Skill> Skills { get; }

        public ToogleRelayCommand Command { get; }

        public ICommand RemoveSkill { get; }

        public ICommand ChangeSkill { get; }

        public ICommand RemoveSkills { get; }

        public ICommand TestCommand { get; set; }

        public ISimpleCommand AddOneYear { get; set; }
    }
}
