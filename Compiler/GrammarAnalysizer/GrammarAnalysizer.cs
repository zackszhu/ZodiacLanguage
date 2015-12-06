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

        private Grammar grammar;
        private LanguageData language;
        private Parser parser;
        private ParseTree parseTree;
        public ParseTree ParseTree
        {
            get { return parseTree; }
        }

        GrammarLoader grammarLoader = new GrammarLoader();
        

        public GrammarAnalysizer() {
            //grammarLoader.AssemblyUpdated += GrammarAssemblyUpdated;
            LoadGrammar();
            CreateParser();
        }

        private void LoadGrammar() {
            var dlgSelectAssembly = new System.Windows.Forms.OpenFileDialog();
            dlgSelectAssembly.DefaultExt = "dll";
            dlgSelectAssembly.Filter = "DLL files|*.dll";
            dlgSelectAssembly.Title = "Select Grammar Assembly ";
            if (dlgSelectAssembly.ShowDialog() != DialogResult.OK) return;
            string location = dlgSelectAssembly.FileName;
            if (string.IsNullOrEmpty(location)) return;
            grammarLoader.SelectedGrammar = SelectGrammars(location);
            if (grammarLoader.SelectedGrammar == null) return;
            grammar = grammarLoader.CreateGrammar();
        }
        private void CreateParser()
        {
            parseTree = null;
            language = new LanguageData(grammar);
            parser = new Parser(language);
        }

        private GrammarItem SelectGrammars(string assemblyPath)
        {
            var fromGrammars = LoadGrammars(assemblyPath);
            if (fromGrammars == null)
                return null;
            //fill the listbox and show the form
            GrammarItem result = fromGrammars[0];
            return result;
        }

        private GrammarItemList LoadGrammars(string assemblyPath)
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
            if (parser == null || !parser.Language.CanParse()) return;
            parseTree = null;
            GC.Collect(); //to avoid disruption of perf times with occasional collections
            parser.Context.TracingEnabled = false;//parsetrace not needed for us
            try {
                parser.Parse(code, "<source>");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally {
                parseTree = parser.Context.CurrentParseTree;
            }
        }

        public void ShowParseTree()
        {
            if (parseTree == null) return;
            AddParseNodeRec(0, parseTree.Root);
        }

        private void AddParseNodeRec(int depth, ParseTreeNode node)
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
            //var t = " "*6;
            for (int i = 0; i < depth; i++) Console.Write("  ");

            if (node.Token != null)
            {
                Console.WriteLine(node.Token.Value + " " + node.Token.Terminal.ToString());
            }
            else
               Console.WriteLine(node.Term.Name );

            

           // Console.WriteLine(node.ToString());
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(depth+1, child);
        }

       
        

    }
}