﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.AI.LanguageGeneration.Tests
{
    [TestClass]
    public class LGFileDiagnosticTest
    {
        [TestMethod]
        public void TestConditionFormatError()
        {
            var diagnostics = GetDiagnostics("ConditionFormatError.lg");

            Assert.AreEqual(10, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.NotEndWithElseInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains(LGErrors.InvalidExpressionInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[2].Severity);
            Assert.IsTrue(diagnostics[2].Message.Contains(LGErrors.MultipleIfInCondition));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[3].Severity);
            Assert.IsTrue(diagnostics[3].Message.Contains(LGErrors.NotEndWithElseInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[4].Severity);
            Assert.IsTrue(diagnostics[4].Message.Contains(LGErrors.ExtraExpressionInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[5].Severity);
            Assert.IsTrue(diagnostics[5].Message.Contains(LGErrors.MultipleIfInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[6].Severity);
            Assert.IsTrue(diagnostics[6].Message.Contains(LGErrors.InvalidMiddleInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[7].Severity);
            Assert.IsTrue(diagnostics[7].Message.Contains(LGErrors.InvalidWhitespaceInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[8].Severity);
            Assert.IsTrue(diagnostics[8].Message.Contains(LGErrors.InvalidWhitespaceInCondition));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[9].Severity);
            Assert.IsTrue(diagnostics[9].Message.Contains(LGErrors.NotStartWithIfInCondition));
        }

        [TestMethod]
        public void TestDuplicatedTemplates()
        {
            var diagnostics = GetDiagnostics("DuplicatedTemplates.lg");

            Assert.AreEqual(2, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.DuplicatedTemplateInSameTemplate("template1")));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains(LGErrors.DuplicatedTemplateInSameTemplate("template1")));

            var lgFile = GetLGFile("DuplicatedTemplates.lg");
            var allDiagnostics = lgFile.AllDiagnostics;

            Assert.AreEqual(4, allDiagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, allDiagnostics[0].Severity);
            Assert.IsTrue(allDiagnostics[0].Message.Contains(LGErrors.DuplicatedTemplateInSameTemplate("template1")));
            Assert.AreEqual(DiagnosticSeverity.Error, allDiagnostics[1].Severity);
            Assert.IsTrue(allDiagnostics[1].Message.Contains(LGErrors.DuplicatedTemplateInSameTemplate("template1")));
            Assert.AreEqual(DiagnosticSeverity.Error, allDiagnostics[2].Severity);
            Assert.IsTrue(allDiagnostics[2].Message.Contains("Duplicated definitions found for template: 'basicTemplate'"));
            Assert.AreEqual(DiagnosticSeverity.Error, allDiagnostics[3].Severity);
            Assert.IsTrue(allDiagnostics[3].Message.Contains("Duplicated definitions found for template: 'basicTemplate2'"));
        }

        [TestMethod]
        public void TestDuplicatedTemplatesInImportFiles()
        {
            var diagnostics = GetDiagnostics("DuplicatedTemplatesInImportFiles.lg");

            Assert.AreEqual(2, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains("Duplicated definitions found for template: 'basicTemplate'"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains("Duplicated definitions found for template: 'basicTemplate2'"));
        }

        [TestMethod]
        public void TestEmptyLGFile()
        {
            var diagnostics = GetDiagnostics("EmptyLGFile.lg");

            Assert.AreEqual(1, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.NoTemplate));
        }

        [TestMethod]
        public void TestEmptyTemplate()
        {
            var diagnostics = GetDiagnostics("EmptyTemplate.lg");

            Assert.AreEqual(1, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.NoTemplateBody("template")));
        }

        [TestMethod]
        public void TestErrorStructuredTemplate()
        {
            var diagnostics = GetDiagnostics("ErrorStructuredTemplate.lg");

            Assert.AreEqual(5, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.InvalidStrucBody));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains(LGErrors.EmptyStrucContent));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[2].Severity);
            Assert.IsTrue(diagnostics[2].Message.Contains("Error occurred when parsing expression 'NOTemplate()'. NOTemplate does not have an evaluator"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[3].Severity);
            Assert.IsTrue(diagnostics[3].Message.Contains("Error occurred when parsing expression 'NOTemplate()'. NOTemplate does not have an evaluator"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[4].Severity);
            Assert.IsTrue(diagnostics[4].Message.Contains(LGErrors.InvalidStrucName));
        }

        [TestMethod]
        public void TestErrorTemplateName()
        {
            var diagnostics = GetDiagnostics("ErrorTemplateName.lg");

            Assert.AreEqual(5, diagnostics.Count);
            foreach (var diagnostic in diagnostics)
            {
                Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
                Assert.IsTrue(diagnostic.Message.Contains(LGErrors.InvalidTemplateName));
            }
        }

        [TestMethod]
        public void TestInvalidLGFileImportPath()
        {
            var diagnostics = GetDiagnostics("InvalidLGFileImportPath.lg");

            Assert.AreEqual(1, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains("Could not find file"));
        }

        [TestMethod]
        public void TestLgTemplateFunctionError()
        {
            var diagnostics = GetDiagnostics("LgTemplateFunctionError.lg");

            Assert.AreEqual(2, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains("Error occurred when parsing expression 'NotExistTemplate()'. NotExistTemplate does not have an evaluator"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains("Error occurred when parsing expression 'template5('hello', 'world')'. arguments mismatch for template 'template5'. Expecting '1' arguments, actual '2'."));
        }

        [TestMethod]
        public void TestMultiLineVariation()
        {
            var diagnostics = GetDiagnostics("MultiLineVariation.lg");

            Assert.AreEqual(1, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.NoEndingInMultiline));
        }

        [TestMethod]
        public void TestNoNormalTemplateBody()
        {
            var diagnostics = GetDiagnostics("NoNormalTemplateBody.lg");

            Assert.AreEqual(3, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.NotEndWithElseInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains(LGErrors.MissingTemplateBodyInCondition));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[2].Severity);
            Assert.IsTrue(diagnostics[2].Message.Contains(LGErrors.MissingTemplateBodyInCondition));
        }

        [TestMethod]
        public void TestNoTemplateRef()
        {
            var diagnostics = GetDiagnostics("NoTemplateRef.lg");

            Assert.AreEqual(3, diagnostics.Count);

            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains("Error occurred when parsing expression 'templateRef()'. templateRef does not have an evaluator"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains("Error occurred when parsing expression 'templateRef(a)'. templateRef does not have an evaluator"));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[2].Severity);
            Assert.IsTrue(diagnostics[2].Message.Contains("Error occurred when parsing expression 'templateRefInMultiLine()'. templateRefInMultiLine does not have an evaluator"));
        }

        [TestMethod]
        public void TestSwitchCaseFormatError()
        {
            var diagnostics = GetDiagnostics("SwitchCaseFormatError.lg");

            Assert.AreEqual(14, diagnostics.Count);
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[0].Severity);
            Assert.IsTrue(diagnostics[0].Message.Contains(LGErrors.InvalidWhitespaceInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[1].Severity);
            Assert.IsTrue(diagnostics[1].Message.Contains(LGErrors.MultipleSwithStatementInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[2].Severity);
            Assert.IsTrue(diagnostics[2].Message.Contains(LGErrors.NotStartWithSwitchInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[3].Severity);
            Assert.IsTrue(diagnostics[3].Message.Contains(LGErrors.MissingCaseInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[4].Severity);
            Assert.IsTrue(diagnostics[4].Message.Contains(LGErrors.InvalidStatementInMiddlerOfSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[5].Severity);
            Assert.IsTrue(diagnostics[5].Message.Contains(LGErrors.NotEndWithDefaultInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[6].Severity);
            Assert.IsTrue(diagnostics[6].Message.Contains(LGErrors.ExtraExpressionInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[7].Severity);
            Assert.IsTrue(diagnostics[7].Message.Contains(LGErrors.MissingTemplateBodyInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[8].Severity);
            Assert.IsTrue(diagnostics[8].Message.Contains(LGErrors.ExtraExpressionInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[9].Severity);
            Assert.IsTrue(diagnostics[9].Message.Contains(LGErrors.MissingTemplateBodyInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[10].Severity);
            Assert.IsTrue(diagnostics[10].Message.Contains(LGErrors.InvalidExpressionInSwiathCase));
            Assert.AreEqual(DiagnosticSeverity.Error, diagnostics[11].Severity);
            Assert.IsTrue(diagnostics[11].Message.Contains(LGErrors.ExtraExpressionInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[12].Severity);
            Assert.IsTrue(diagnostics[12].Message.Contains(LGErrors.MissingCaseInSwitchCase));
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[13].Severity);
            Assert.IsTrue(diagnostics[13].Message.Contains(LGErrors.NotEndWithDefaultInSwitchCase));
        }

        [TestMethod]
        public void TestLoopDetected()
        {
            var lgFile = GetLGFile("LoopDetected.lg");
            var exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("wPhrase"));
            Assert.IsTrue(exception.Message.Contains(LGErrors.LoopDetected));

            exception = Assert.ThrowsException<Exception>(() => lgFile.AnalyzeTemplate("wPhrase"));
            Assert.IsTrue(exception.Message.Contains(LGErrors.LoopDetected));
        }

        [TestMethod]
        public void AddTextWithWrongId()
        {
            var diagnostics = LGParser.ParseText("[import](xx.lg) \r\n # t \n - hi", "a.lg").Diagnostics;
            Assert.AreEqual(1, diagnostics.Count);
            Assert.IsTrue(diagnostics[0].Message.Contains("Could not find file"));
        }

        [TestMethod]
        public void TestErrorExpression()
        {
            var lgFile = GetLGFile("ErrorExpression.lg");
            var exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("template1"));
            Assert.IsTrue(exception.Message.Contains("Error occurred when evaluating"));
        }

        [TestMethod]
        public void TestRunTimeErrors()
        {
            var lgFile = GetLGFile("RunTimeErrors.lg");

            var exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("template1"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [template1]  Error occurred when evaluating '-I want ${dialog.abc}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("prebuilt1"));
            Assert.AreEqual("'dialog.abc' evaluated to null.[prebuilt1]  Error occurred when evaluating '-I want ${foreach(dialog.abc, item, template1())}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("template2"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [template1]  Error occurred when evaluating '-I want ${dialog.abc}'. [template2]  Error occurred when evaluating '-With composition ${template1()}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("conditionalTemplate1", new { dialog = true }));
            Assert.AreEqual("'dialog.abc' evaluated to null. [template1]  Error occurred when evaluating '-I want ${dialog.abc}'. [conditionalTemplate1] Condition '${dialog}':  Error occurred when evaluating '-I want ${template1()}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("conditionalTemplate2"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [conditionalTemplate2] Condition '${dialog.abc}': Error occurred when evaluating '-IF :${dialog.abc}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("structured1"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [structured1] Property 'Text': Error occurred when evaluating 'Text=I want ${dialog.abc}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("structured2"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [template1]  Error occurred when evaluating '-I want ${dialog.abc}'. [structured2] Property 'Text': Error occurred when evaluating 'Text=I want ${template1()}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("structured3"));
            Assert.AreEqual("'dialog.abc' evaluated to null. [template1]  Error occurred when evaluating '-I want ${dialog.abc}'. [structured2] Property 'Text': Error occurred when evaluating 'Text=I want ${template1()}'. [structured3]  Error occurred when evaluating '${structured2()}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("switchcase1", new { turn = new { testValue = 1 } }));
            Assert.AreEqual("'dialog.abc' evaluated to null. [switchcase1] Case '${1}': Error occurred when evaluating '-I want ${dialog.abc}'. ", exception.Message);

            exception = Assert.ThrowsException<Exception>(() => lgFile.EvaluateTemplate("switchcase2", new { turn = new { testValue = 0 } }));
            Assert.AreEqual("'dialog.abc' evaluated to null. [switchcase2] Case 'Default': Error occurred when evaluating '-I want ${dialog.abc}'. ", exception.Message);
        }

        [TestMethod]
        public void TestExpressionFormatError()
        {
            var diagnostics = GetDiagnostics("ExpressionFormatError.lg");
            Assert.AreEqual(1, diagnostics.Count);
            Assert.IsTrue(diagnostics[0].Message.Contains("Close } is missing in Expression"));
        }

        private string GetExceptionExampleFilePath(string fileName)
        {
            return AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + "ExceptionExamples" + Path.DirectorySeparatorChar + fileName;
        }

        private LGFile GetLGFile(string fileName)
        {
            var filePath = GetExceptionExampleFilePath(fileName);
            return LGParser.ParseFile(filePath);
        }

        private IList<Diagnostic> GetDiagnostics(string fileName)
        {
            var filePath = GetExceptionExampleFilePath(fileName);
            return LGParser.ParseFile(filePath).Diagnostics;
        }
    }
}
