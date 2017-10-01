using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Test.Helper;
using Xunit;

namespace Neutronium.Core.Test.Binding.Listeners
{
    public class SilenterTest : IDisposable
    {
        private readonly ClrTypesTestViewModel _SimpleObservable;
        private readonly ListenerRegister<INotifyPropertyChanged> _ListenerRegister;
        private readonly Listener _Listener;
        private PropertyChangedSilenter _Silenter;

        private class Listener
        {
            public List<Tuple<object, string>> Events { get; } = new List<Tuple<object, string>>();

            internal void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Events.Add(Tuple.Create(sender, e.PropertyName));
            }
        }

        public SilenterTest()
        {
            _SimpleObservable = new ClrTypesTestViewModel();
            _Listener = new Listener();
            _ListenerRegister = new ListenerRegister<INotifyPropertyChanged>(ob => ob.PropertyChanged += _Listener.OnPropertyChanged, ob => ob.PropertyChanged -= _Listener.OnPropertyChanged);
            _ListenerRegister.OnEnter(_SimpleObservable);
        }

        [Fact]
        public void Silenter_silents_events_corresponding_to_property()
        {
            _Silenter = Silenter.GetSilenter(_ListenerRegister, _SimpleObservable, "Int16");
            _SimpleObservable.Int16 = 55;
            _Listener.Events.Should().BeEmpty();
        }

        [Fact]
        public void Silenter_does_not_silent_events_corresponding_to_other_property()
        {
            _Silenter = Silenter.GetSilenter(_ListenerRegister, _SimpleObservable, "Int16");
            _SimpleObservable.Int32 = 5;
            _Listener.Events.Select(ev => ev.Item1).Should().Equal(_SimpleObservable);
            _Listener.Events.Select(ev => ev.Item2).Should().Equal("Int32");
        }

        [Fact]
        public void Silenter_silents_events_corresponding_to_property_until_dispose()
        {
            _Silenter = Silenter.GetSilenter(_ListenerRegister, _SimpleObservable, "Int16");
            _Silenter.Dispose();
            _SimpleObservable.Int16 = 55;
            _Listener.Events.Select(ev => ev.Item1).Should().Equal(_SimpleObservable);
            _Listener.Events.Select(ev => ev.Item2).Should().Equal("Int16");
        }

        [Fact]
        public void Silenter_unlisten_after_dispose()
        {
            _Silenter = Silenter.GetSilenter(_ListenerRegister, _SimpleObservable, "Int16");
            _Silenter.Dispose();

            _SimpleObservable.ListenerCount.Should().Be(1);

            _ListenerRegister.OnExit(_SimpleObservable);
            _SimpleObservable.ListenerCount.Should().Be(0);
        }

        public void Dispose()
        {
            _Silenter.Dispose();
        }
    }
}

