﻿using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;
using System.Windows.Forms;
using Irony.GrammarExplorer;
using System.Reflection;
using System.IO;

namespace Zodiac {
    public class GrammarAnalysizer {

        private Grammar _grammar;
        private LanguageData language;
        private Parser _parser;
        private ParseTree parseTree;
        GrammarLoader grammarLoader = new GrammarLoader();


        public GrammarAnalysizer() {
            //grammarLoader.AssemblyUpdated += GrammarAssemblyUpdated;
        }

        public void load()
        {
            var dlgSelectAssembly = new System.Windows.Forms.OpenFileDialog();
            dlgSelectAssembly.DefaultExt = "dll";
            dlgSelectAssembly.Filter = "DLL files|*.dll";
            dlgSelectAssembly.Title = "Select Grammar Assembly ";
            if (dlgSelectAssembly.ShowDialog() != DialogResult.OK) return;
            string location = dlgSelectAssembly.FileName;
            if (string.IsNullOrEmpty(location)) return;
            grammarLoader.SelectedGrammar = SelectGrammars(location);
            if (grammarLoader.SelectedGrammar == null) return;
            _grammar = grammarLoader.CreateGrammar();
            CreateParser();
        }
        private void CreateParser()
        {
            parseTree = null;

            language = new LanguageData(_grammar);
            _parser = new Parser(language);
        }

        public static GrammarItem SelectGrammars(string assemblyPath)
        {
            var fromGrammars = LoadGrammars(assemblyPath);
            if (fromGrammars == null)
                return null;
            //fill the listbox and show the form
            GrammarItem result = fromGrammars[0];
            return result;
        }

        private static GrammarItemList LoadGrammars(string assemblyPath)
        {
            Assembly asm = null;
            try
            {
                asm = GrammarLoader.LoadAssembly(assemblyPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load assembly: " + ex.Message);
                return null;
            }
            var types = asm.GetTypes();
            var grammars = new GrammarItemList();
            foreach (Type t in types)
            {
                if (t.IsAbstract) continue;
                if (!t.IsSubclassOf(typeof(Grammar))) continue;
                grammars.Add(new GrammarItem(t, assemblyPath));
            }
            if (grammars.Count == 0)
            {
                MessageBox.Show("No classes derived from Irony.Grammar were found in the assembly.");
                return null;
            }
            return grammars;
        }

       
        public void ParseSample(string code) {
            if (_parser == null || !_parser.Language.CanParse()) return;
            parseTree = null;
            GC.Collect(); //to avoid disruption of perf times with occasional collections
            _parser.Context.TracingEnabled = false;//parsetrace not needed for us
            try {
                _parser.Parse(code, "<source>");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally {
                parseTree = _parser.Context.CurrentParseTree;
            }
        }

        public void ShowParseTree()
        {
            if (parseTree == null) return;
            AddParseNodeRec(parseTree.Root);
        }

        private void AddParseNodeRec(ParseTreeNode node)
        {
            if (node == null) return;
            BnfTerm term = node.Term;
            string txt = node.ToString();
            if(term == null)
            {
                txt = "NullTerm";
            }
            else
            {
                txt = term.GetParseNodeCaption(node);
            }
            Console.WriteLine(txt);
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(child);
        }

       
        

    }
}
