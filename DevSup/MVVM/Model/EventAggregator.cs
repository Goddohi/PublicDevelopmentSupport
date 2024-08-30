using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.MVVM.Model
{
    public class EventAggregator
    {
        //이것도 싱글톤 패턴으로 만들어버려서 모든 곳에서 동일하게 사용하게 해버릴까?
        public event Action SpecialEventOccurred;

        public void PublishSpecialEvent()
        {
            SpecialEventOccurred?.Invoke();
        }
        public event Action TabsaveEventOccurred;
        public void TabsaveEvent()
        {
            TabsaveEventOccurred?.Invoke();
        }
    }
}
