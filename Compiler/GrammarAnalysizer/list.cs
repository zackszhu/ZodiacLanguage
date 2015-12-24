using System;
using System.Collections;
using System.Collections.Generic;

namespace GrammarAnalysizer {
    public class list : IEnumerable{
        private List<int> _elements = new List<int>();
        private int _offset;

        //public int Length => _elements.Count;
        public int Length ()
        {
            return _elements.Count;
        }

        public int this[int index] {
            get { return _elements[index - _offset]; }
            set { _elements[index - _offset] = value; }
        }

        public list() {
            _offset = 0;
        }

        public list(int start, int end) {
            _elements = new List<int>();
            for (var i = start; i < end; i++) {
                _elements.Add(i);               
            }                                   
        }                                       
                                                
        public list(int start, int end, int value) {     
            _offset = start;                                
            for (var i = 0; i < end - start; i++) {  
                _elements.Add(value);
            }
        }

        public list(int start, int end, int startValue, int endValue) {
            _offset = start;
            for (var i = 0; i < end - start; i++) {
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
                _elements = new List<int>(l1._elements),
                _offset = l1._offset
            };
            Console.Write(l1.ToString() + "+"  +l2.ToString() + "=" );
            ret._elements.AddRange(l2._elements);
            Console.WriteLine(ret);
            return ret;
        }

        public override string ToString() {
            var str = "[";
            for (var i = 0; i < _elements.Count - 1; i++) {
                str += _elements[i].ToString();
                str += ", ";
            }

            if (_elements.Count > 0)
                str += _elements[_elements.Count - 1].ToString();
            str += "]";
            return str;
        }

        public IEnumerator GetEnumerator() {
            return _elements.GetEnumerator();
        }
    }
}
