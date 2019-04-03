using Assembler.CodeGenerator.AdvancedForm.Form;
using Assembler.CodeGenerator.AdvancedForm.Pages;
using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm
{
    public class AdvancedFormInstallCodeGenerator
    {
        private Config _config;
        private BuildType _buildType;

        public AdvancedFormInstallCodeGenerator(Config config, BuildType buildType)
        {
            _config = config;
            _buildType = buildType;
        }

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            var res = new StringBuilder();
            res.AppendLine(Page1Generator.Generate());
            res.AppendLine(Page2Generator.Generate(_config, _buildType));

            var page3 = Page3Generator.Generate(_config, _buildType);

            res.AppendLine(page3.Code);
            res.AppendLine(Page4Generator.Generate(_config));

            res.AppendLine(FormGenerator.Generate(_config, _buildType));

            return (res.ToString(), page3.Resources);
        }
    }
}
