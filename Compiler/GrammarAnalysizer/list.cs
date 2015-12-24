using System;
using System.Collections;
using System.Collections.Generic;

namespace GrammarAnalysizer {
    public class list : IEnumerable{
        private List<int> _elements = new List<int>();
        private int _offset;

        public int Length => _elements.Count;

        public int this[int index] {
            get { return _elements[index - _offset]; }
            set { _elements[index - _offset] = value; }
        }

        public list() {
            _offset = 0;
        }

        public list(int start, int end) {
            _elements = new List<int>();
            for (var i = start; i <= end; i++) {
                _elements.Add(i);               
            }                                   
        }                                       
                                                
        public list(int start, int end, int value) {     
            _offset = start;                                
            for (var i = 0; i <= end - start; i++) {  
                _elements.Add(value);
            }
        }

        public list(int start, int end, int startValue, int endValue) {
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _elements.Add(startValue + i > endValue ? 0 : startValue + i);
            }
        }

        public list(string value) {
            foreach (var ch in value) {
                _elements.Add(ch);
            }
        }

        public void Append(int value) {
            _elements.Add(value);
        }

        public static list operator +(list l1, list l2) {
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
