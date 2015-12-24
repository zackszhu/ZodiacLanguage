using System;
using System.Collections;

namespace GrammarAnalysizer {
    public class list : IEnumerable{
        private ArrayList _elements = new ArrayList();
        private int _offset;

        public int Length => _elements.Count;

        public Type ElmtType;

        public object this[int index] {
            get { return _elements[index - _offset]; }
            set { _elements[index - _offset] = value; }
        }

        public list() {
            _elements = new ArrayList();
            _offset = 0;
        }

        public list(int start, int end) {
            for (var i = start; i < end; i++) {
                _elements.Add(i);
            }
            ElmtType = typeof (int);
        }

        public list(int start, int end, object value) {
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _elements[i] = value;
            }
            ElmtType = value.GetType();
        }

        public list(int start, int end, int startValue, int endValue) {
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _elements[i] = startValue + i > endValue ? 0 : startValue + i;
            }
            ElmtType = typeof (int);
        }

        public list(string value) {
            foreach (var ch in value) {
                _elements.Add(ch);
            }
            ElmtType = typeof (char);
        }

        public void Append(object value) {
            _elements.Add(value);
        }

        public static list operator +(list l1, list l2) {
            // @TODO type check
            var ret = new list() {
                _elements = l1._elements,
                _offset = l1._offset
            };
            ret._elements.AddRange(l2._elements);
            return ret;
        }

        public override string ToString() {
            var str = "[";
            foreach (var element in _elements) {
                str += element.ToString();
                str += ", ";
            }
            str += "]";
            return str;
        }

        public IEnumerator GetEnumerator() {
            return _elements.GetEnumerator();
        }
    }
}
