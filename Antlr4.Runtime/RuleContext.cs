/*
 * [The "BSD license"]
 *  Copyright (c) 2013 Terence Parr
 *  Copyright (c) 2013 Sam Harwell
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *  1. Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The name of the author may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 *  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 *  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 *  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 *  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 *  THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Sharpen;

namespace Antlr4.Runtime
{
    /// <summary>A rule context is a record of a single rule invocation.</summary>
    /// <remarks>
    /// A rule context is a record of a single rule invocation. It knows
    /// which context invoked it, if any. If there is no parent context, then
    /// naturally the invoking state is not valid.  The parent link
    /// provides a chain upwards from the current rule invocation to the root
    /// of the invocation tree, forming a stack. We actually carry no
    /// information about the rule associated with this context (except
    /// when parsing). We keep only the state number of the invoking state from
    /// the ATN submachine that invoked this. Contrast this with the s
    /// pointer inside ParserRuleContext that tracks the current state
    /// being "executed" for the current rule.
    /// The parent contexts are useful for computing lookahead sets and
    /// getting error information.
    /// These objects are used during parsing and prediction.
    /// For the special case of parsers and tree parsers, we use the subclass
    /// ParserRuleContext.
    /// </remarks>
    /// <seealso cref="ParserRuleContext">ParserRuleContext</seealso>
    public class RuleContext : IRuleNode
    {
        /// <summary>What context invoked this rule?</summary>
        public Antlr4.Runtime.RuleContext parent;

        /// <summary>
        /// What state invoked the rule associated with this context?
        /// The "return address" is the followState of invokingState
        /// If parent is null, this should be -1.
        /// </summary>
        /// <remarks>
        /// What state invoked the rule associated with this context?
        /// The "return address" is the followState of invokingState
        /// If parent is null, this should be -1.
        /// </remarks>
        public int invokingState = -1;

        public RuleContext()
        {
        }

        public RuleContext(Antlr4.Runtime.RuleContext parent, int invokingState)
        {
            this.parent = parent;
            //if ( parent!=null ) System.out.println("invoke "+stateNumber+" from "+parent);
            this.invokingState = invokingState;
        }

        public static Antlr4.Runtime.RuleContext GetChildContext(Antlr4.Runtime.RuleContext
             parent, int invokingState)
        {
            return new Antlr4.Runtime.RuleContext(parent, invokingState);
        }

        public virtual int Depth()
        {
            int n = 0;
            Antlr4.Runtime.RuleContext p = this;
            while (p != null)
            {
                p = p.parent;
                n++;
            }
            return n;
        }

        /// <summary>
        /// A context is empty if there is no invoking state; meaning nobody call
        /// current context.
        /// </summary>
        /// <remarks>
        /// A context is empty if there is no invoking state; meaning nobody call
        /// current context.
        /// </remarks>
        public virtual bool IsEmpty()
        {
            return invokingState == -1;
        }

        public virtual Interval SourceInterval
        {
            get
            {
                // satisfy the ParseTree / SyntaxTree interface
                return Interval.Invalid;
            }
        }

        public virtual Antlr4.Runtime.RuleContext RuleContext
        {
            get
            {
                return this;
            }
        }

        public virtual Antlr4.Runtime.RuleContext Parent
        {
            get
            {
                return parent;
            }
        }

        public virtual Antlr4.Runtime.RuleContext Payload
        {
            get
            {
                return this;
            }
        }

        /// <summary>Return the combined text of all child nodes.</summary>
        /// <remarks>
        /// Return the combined text of all child nodes. This method only considers
        /// tokens which have been added to the parse tree.
        /// <p>
        /// Since tokens on hidden channels (e.g. whitespace or comments) are not
        /// added to the parse trees, they will not appear in the output of this
        /// method.
        /// </remarks>
        public virtual string GetText()
        {
            if (ChildCount == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < ChildCount; i++)
            {
                builder.Append(GetChild(i).GetText());
            }
            return builder.ToString();
        }

        public virtual int GetRuleIndex()
        {
            return -1;
        }

        public virtual IParseTree GetChild(int i)
        {
            return null;
        }

        public virtual int ChildCount
        {
            get
            {
                return 0;
            }
        }

        public virtual T Accept<T, _T1>(IParseTreeVisitor<_T1> visitor) where _T1:T
        {
            return visitor.VisitChildren(this);
        }

        /// <summary>
        /// Print out a whole tree, not just a node, in LISP format
        /// (root child1 ..
        /// </summary>
        /// <remarks>
        /// Print out a whole tree, not just a node, in LISP format
        /// (root child1 .. childN). Print just a node if this is a leaf.
        /// We have to know the recognizer so we can get rule names.
        /// </remarks>
        public virtual string ToStringTree(Parser recog)
        {
            return Trees.ToStringTree(this, recog);
        }

        /// <summary>
        /// Print out a whole tree, not just a node, in LISP format
        /// (root child1 ..
        /// </summary>
        /// <remarks>
        /// Print out a whole tree, not just a node, in LISP format
        /// (root child1 .. childN). Print just a node if this is a leaf.
        /// </remarks>
        public virtual string ToStringTree(IList<string> ruleNames)
        {
            return Trees.ToStringTree(this, ruleNames);
        }

        public virtual string ToStringTree()
        {
            return ToStringTree((IList<string>)null);
        }

        public override string ToString()
        {
            return ToString((IList<string>)null, (Antlr4.Runtime.RuleContext)null);
        }

        public string ToString<_T0>(Recognizer<_T0> recog)
        {
            return ToString(recog, ParserRuleContext.EmptyContext);
        }

        public string ToString(IList<string> ruleNames)
        {
            return ToString(ruleNames, null);
        }

        // recog null unless ParserRuleContext, in which case we use subclass toString(...)
        public virtual string ToString<_T0>(Recognizer<_T0> recog, Antlr4.Runtime.RuleContext
             stop)
        {
            string[] ruleNames = recog != null ? recog.RuleNames : null;
            IList<string> ruleNamesList = ruleNames != null ? Arrays.AsList(ruleNames) : null;
            return ToString(ruleNamesList, stop);
        }

        public virtual string ToString(IList<string> ruleNames, Antlr4.Runtime.RuleContext
             stop)
        {
            StringBuilder buf = new StringBuilder();
            Antlr4.Runtime.RuleContext p = this;
            buf.Append("[");
            while (p != null && p != stop)
            {
                if (ruleNames == null)
                {
                    if (!p.IsEmpty())
                    {
                        buf.Append(p.invokingState);
                    }
                }
                else
                {
                    int ruleIndex = p.GetRuleIndex();
                    string ruleName = ruleIndex >= 0 && ruleIndex < ruleNames.Count ? ruleNames[ruleIndex
                        ] : Sharpen.Extensions.ToString(ruleIndex);
                    buf.Append(ruleName);
                }
                if (p.parent != null && (ruleNames != null || !p.parent.IsEmpty()))
                {
                    buf.Append(" ");
                }
                p = p.parent;
            }
            buf.Append("]");
            return buf.ToString();
        }
    }
}
