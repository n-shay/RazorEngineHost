namespace RazorEngineHost.Test
{
    using System;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RazorEngineHost.Templating;

    [TestClass]
    public class MicroRazorCompilerTests
    {
        [TestMethod]
        public void SimpleTemplate()
        {
            // The template
            const string Templ = "<p>@Environment.Version</p>";
            var expected = "<p>" + Environment.Version + "</p>";

            // Arrange
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected);
        }

        [TestMethod]
        public void HtmlEncoding()
        {
            // The template
            const string Templ = "<p>@(\"<p>\")</p>";
            var expected = "<p>&lt;p&gt;</p>";

            // Arrange
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected);
        }

        [TestMethod]
        public void LiteralAttribute()
        {
            // The template
            const string Templ = "<p class=\"foo\"/>";
            var expected = "<p class=\"foo\"/>";

            // Arrange
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected);
        }

        [TestMethod]
        public void NonLiteralAttribute()
        {
            // The template
            const string Templ = "<p class=\"@Environment.Version\"/>";
            var expected = "<p class=\"" + Environment.Version + "\"/>";

            // Arrange
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected);
        }

        [TestMethod]
        public void ConditionalAttribute()
        {
            // The template
            const string Templ = "@{object f = null;}<p class=\"foo @f bar\" />";
            var expected = "<p class=\"foo bar\" />";

            // Arrange
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected);
        }

        [TestMethod]
        public void DynamicModelTemplate()
        {
            // The template
            const string Templ = "Hello @Model.Name";
            var expected = "Hello Shay";

            // Arrange
            var model = new
                {
                    Name = "Shay"
                };
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected, model);
        }

        [TestMethod]
        public void DynamicModelWithChildTemplate()
        {
            // The template
            const string Templ = "Hello @Model.Name (@Model.Child.Email)";
            var expected = "Hello Shay (test@test.com)";

            // Arrange
            var model = new
                {
                    Name = "Shay",
                    Child = new
                        {
                            Email = "test@test.com"
                        }
                };
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected, model);
        }

        [TestMethod]
        public void SimpleModelTemplate()
        {
            // The template
            const string Templ = "Hello @Model.Name";
            var expected = "Hello Shay";

            // Arrange
            var model = new TestModel
                {
                    Name = "Shay"
                };
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected, model, typeof(TestModel));
        }

        [TestMethod]
        public void ModelWithChildTemplate()
        {
            // The template
            const string Templ = "Hello @Model.Child.Name (@Model.D.ToString(\"F2\"))";
            var expected = "Hello Shay (1.23)";

            // Arrange
            var model = new TestParentModel
                {
                    D = 1.234,
                    Child = new TestModel
                        {
                            Name = "Shay"
                        }
                };
            var service = RazorEngineHost.Create(c => c.WithBaseTemplateType(typeof(TemplateBase<>)));

            // Act
            this.CompileRunAndAssert(service, Templ, expected, model, typeof(TestParentModel));

        }

        private void CompileRunAndAssert(
            IRazorEngineHost service,
            string templateSource,
            string expected,
            object model = null,
            Type modelType = null)
        {
            var sw = Stopwatch.StartNew();
            var template = service.Compile(templateSource, modelType);
            var ms1 = sw.ElapsedMilliseconds;
            Assert.IsNotNull(template);
            sw.Restart();
            var results = service.Run(template, model: model, modelType: modelType);
            sw.Stop();
            Assert.AreEqual(expected, results);

            this.OutputStats(ms1, sw.ElapsedMilliseconds);
        }

        private void OutputStats(params long[] stats)
        {
            if(stats == null)
                throw new ArgumentNullException();
            var result = "Execution times = [";
            result += string.Join("ms, ", stats);
            result += (stats.Length > 0
                ? "ms"
                : "") + "]";
            Trace.WriteLine(result);
        }

        public class TestModel
        {
            public string Name { get; set; }
        }

        public class TestParentModel
        {
            public double D { get; set; }

            public TestModel Child { get; set; }
        }
    }
}