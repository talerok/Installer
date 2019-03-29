using Assembler.CodeGenerator.AdvancedForm.Form;
using Assembler.CodeGenerator.AdvancedForm.Pages;
using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm
{
    class AdvancedFormInstallCodeGenerator
    {
        private Config _config;

        public AdvancedFormInstallCodeGenerator(Config config)
        {
            _config = config;
        }

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            var res = new StringBuilder();
            res.AppendLine(Page1Generator.Generate());
            res.AppendLine(Page2Generator.Generate(_config));

            var page3 = Page3Generator.Generate(_config);

            res.AppendLine(page3.Code);
            res.AppendLine(Page4Generator.Generate(_config));

            res.AppendLine(FormGenerator.Generate(_config));

            return (res.ToString(), page3.Resources);
        }
    }
}
