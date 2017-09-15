using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public class EntityDescriptorSpliterList 
    {
        public int MaxCount { get; }

        private readonly List<List<ObjectDescriptor>> _ListsObjectDescriptor = new List<List<ObjectDescriptor>>();
        private List<ObjectDescriptor> _CurrentList;
        private int _ParametersCount = 0; 

        public EntityDescriptorSpliterList(int maxCount) 
        {
            PushNew();
            MaxCount = maxCount;
        }

        private void PushNew() 
        {
            _CurrentList = new List<ObjectDescriptor>();
            _ListsObjectDescriptor.Add(_CurrentList);
            _ParametersCount = 0;
        }

        public void Add(ObjectDescriptor element) 
        {
            var maxCountInContext = MaxCount - 2;

            var childrenCount = element.AttributeValues.Length;
            var tentative = _ParametersCount + 1 + childrenCount;
            var delta = tentative - MaxCount;

            if ((delta == -1) || (delta == -2)) 
            {
                _CurrentList.Add(element);
                PushNew();
                return;
            }

            if (delta >= 0) 
            {
                var maxToTake = maxCountInContext - _ParametersCount;
                _CurrentList.Add(element.Take(maxToTake));

                var count = childrenCount - maxToTake;
                var i = 0;
                for (i = 0; i < count / maxCountInContext; i++) 
                {
                    PushNew();
                    _CurrentList.Add(element.Split(maxToTake + maxCountInContext * i, maxCountInContext));
                }
                
                PushNew();
                var skipped = (maxToTake + maxCountInContext * i);
                _ParametersCount = childrenCount - skipped;
                if (skipped < childrenCount) 
                {
                    _CurrentList.Add(element.Skip(skipped));
                    _ParametersCount++;
                }
                return;
            }

            _ParametersCount = tentative;
            _CurrentList.Add(element);
        }
    }
}
