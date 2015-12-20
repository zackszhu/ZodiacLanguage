using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarAnalysizer {
    public class MyList {
        private ArrayList _element = new ArrayList();
        private int _offset = 0;

        public int Length => _element.Count;

        public object this[int index] {
            get { return _element[index - _offset]; }
            set { _element[index - _offset] = value; }
        }

        public MyList(int start, int end) {
            for (int i = start; i < end; i++) {
                _element.Add(i);
            }
        }

        public MyList(int start, int end, object value) {
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _element[i] = value;
            }
        }

        public MyList(int start, int end, int startValue, int endValue) {
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _element[i] = startValue + i > endValue ? 0 : startValue + i;
            }
        }
    }
}
