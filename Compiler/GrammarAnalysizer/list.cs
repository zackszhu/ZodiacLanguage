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
            _offset = 0;
        }

        public list(int start, int end) {
            _elements = new ArrayList();
            for (var i = start; i <= end; i++) {//hehe////////    //        //      ////////////    //        //      ////////////      //
                _elements.Add(i);                           //    //        //      //              //        //      //                //
            }                                               //    //        //      //              //        //      //                //
            ElmtType = typeof (int);                        //    //        //      //              //        //      //                //
        }                                                   //    ////////////      ////////////    ////////////      ////////////      //
                                                            //    //        //      //              //        //      //                //
        public list(int start, int end, object value) {     //    //        //      //              //        //      //
            _offset = start;                                //    //        //      //              //        //      //                //
            for (var i = 0; i <= end - start; i++) {  //hehe//    //        //      ////////////    //        //      ////////////      //
                _elements.Add(value);
            }
            ElmtType = value.GetType();
        }

        public list(int start, int end, int startValue, int endValue) {
            _elements = new ArrayList();
            _offset = start;
            for (var i = 0; i <= end - start; i++) {
                _elements.Add(startValue + i > endValue ? 0 : startValue + i);
            }
            ElmtType = typeof (int);
        }

        public list(string value) {
            _elements = new ArrayList();
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
