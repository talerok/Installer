﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InstallerLib.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("InstallerLib.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Добавление программы {0} в автозапуск, по пути {1}.
        /// </summary>
        internal static string AutoStartCommandDescription {
            get {
                return ResourceManager.GetString("AutoStartCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка валидации пути записи значения автозапуска ({0}).
        /// </summary>
        internal static string AutoStartCommandParamsExceptionMessage {
            get {
                return ResourceManager.GetString("AutoStartCommandParamsExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка записи значения автозапуска в реестр по пути {0}.
        /// </summary>
        internal static string AutoStartCommandRegisterExceptionMessage {
            get {
                return ResourceManager.GetString("AutoStartCommandRegisterExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Очистка папки {0}.
        /// </summary>
        internal static string ClearDirectoryDescription {
            get {
                return ResourceManager.GetString("ClearDirectoryDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка очистки папки {0}.
        /// </summary>
        internal static string ClearDirectoryException {
            get {
                return ResourceManager.GetString("ClearDirectoryException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка установки.
        /// </summary>
        internal static string CommandException {
            get {
                return ResourceManager.GetString("CommandException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выполняю.
        /// </summary>
        internal static string CommandInProc {
            get {
                return ResourceManager.GetString("CommandInProc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка чтения конфигурации установки.
        /// </summary>
        internal static string ConfigReadFail {
            get {
                return ResourceManager.GetString("ConfigReadFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на преравна с ошибкой.
        /// </summary>
        internal static string FailInstall {
            get {
                return ResourceManager.GetString("FailInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Установка {0}. Нажмите любую клавишу..
        /// </summary>
        internal static string InstallComplete {
            get {
                return ResourceManager.GetString("InstallComplete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка установшика.
        /// </summary>
        internal static string InstallerException {
            get {
                return ResourceManager.GetString("InstallerException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Запуск установки.
        /// </summary>
        internal static string InstallStart {
            get {
                return ResourceManager.GetString("InstallStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Обновление значение директории установки {0}.
        /// </summary>
        internal static string SetPathDescription {
            get {
                return ResourceManager.GetString("SetPathDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка обновление значение директории установки ({0}).
        /// </summary>
        internal static string SetPathException {
            get {
                return ResourceManager.GetString("SetPathException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Обновление значения версии программы {0}.
        /// </summary>
        internal static string SetVersionDescription {
            get {
                return ResourceManager.GetString("SetVersionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка обновления значения верссии программы {0}.
        /// </summary>
        internal static string SetVersionException {
            get {
                return ResourceManager.GetString("SetVersionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка конвертации значения реестра {0} в формат {1} по пути {2}.
        /// </summary>
        internal static string SimpleRegisterCommandConvertExceptionMessage {
            get {
                return ResourceManager.GetString("SimpleRegisterCommandConvertExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Добавление значение {0}  в формате {1} в реестр, по пути {2}.
        /// </summary>
        internal static string SimpleRegisterCommandDescription {
            get {
                return ResourceManager.GetString("SimpleRegisterCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка запизи значения {0} в формате {1} по пути {2}.
        /// </summary>
        internal static string SimpleRegisterCommandRegisterExcpetionMessage {
            get {
                return ResourceManager.GetString("SimpleRegisterCommandRegisterExcpetionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка отката изменений значения реестра {0} по пути {2} ({3}).
        /// </summary>
        internal static string SimpleRegisterCommandUndoException {
            get {
                return ResourceManager.GetString("SimpleRegisterCommandUndoException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на завершена успешно.
        /// </summary>
        internal static string SuccesInstall {
            get {
                return ResourceManager.GetString("SuccesInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка разархивирования бандла {0} в папку {1}.
        /// </summary>
        internal static string ZipBundleUnpackerBundleExtractExceptionMessage {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerBundleExtractExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Некоректное имя бандла.
        /// </summary>
        internal static string ZipBundleUnpackerBundleNameExceptionMessage {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerBundleNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка создания папки {0}.
        /// </summary>
        internal static string ZipBundleUnpackerBundlePathExceptionMessage {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerBundlePathExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Отсутствует данные бандла {0}.
        /// </summary>
        internal static string ZipBundleUnpackerDataExceptionMessage {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerDataExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Распаковка архива бандла {0} размером {1} байт в деректорию {2}.
        /// </summary>
        internal static string ZipBundleUnpackerDesription {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerDesription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка отката измененй бандла {0} ({1}).
        /// </summary>
        internal static string ZipBundleUnpackerUndoException {
            get {
                return ResourceManager.GetString("ZipBundleUnpackerUndoException", resourceCulture);
            }
        }
    }
}
