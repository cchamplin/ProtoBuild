using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;
using ProtoBuild.Types;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements.Requirements
{
    [ChildTypes(new Type[] { typeof(Requirement) })]
    internal class RegistryRequirement : XBDElement, IRequirement
    {
       
        protected String Comparitor { get; set; }
        protected String MatchValue { get; set; }
        protected String MinMatch { get; set; }
        protected String MaxMatch { get; set; }
        protected String SubKey { get; set; }
        protected RegistryItem RegistryItem { get; set; }
        protected bool PatternMatch { get; set; }


        private RegistryRequirement()
            : base(null)
        {
        }
        public RegistryRequirement(XBDDef def) 
            : base(def)
        {
        }

        public bool Validate(List<Errors.RequirementError> errors)
        {

            bool validationResult = true;
            // Sub Key?
            



            if (RegistryItem.IsNull(SubKey))
            {
                if (Comparitor.ToLower() == "null")
                {
                    validationResult = true;
                }
                else if (Comparitor == "=" && MatchValue.ToLower() == "null")
                {
                    validationResult = true;
                }
            }
            else
            {
                object result = RegistryItem.GetValue(this.SubKey);
                var resultType = RegistryItem.GetKind(this.SubKey);


                double minMatchDouble = double.MinValue;
                double maxMatchDouble = double.MaxValue;
               
                if (string.IsNullOrEmpty(MatchValue))
                {
                    if (!string.IsNullOrEmpty(MinMatch))
                    {
                        if (!double.TryParse(MinMatch, out minMatchDouble))
                        {
                            errors.Add(new Errors.RequirementError { Message = "Min Value must be numeric", SourceLine = base.LineContext });
                            validationResult = false;
                        }

                    }
                    if (!string.IsNullOrEmpty(MaxMatch))
                    {
                        if (!double.TryParse(MaxMatch, out maxMatchDouble))
                        {
                            errors.Add(new Errors.RequirementError { Message = "Max Value must be numeric", SourceLine = base.LineContext });
                            validationResult = false;
                        }

                    }
                }

                int matchInt;
                long matchLong;
                double matchDouble;
                object matchObject = null;
                switch (resultType)
                {
                    case RegistryValueKind.DWord:
                        if (!string.IsNullOrEmpty(MatchValue))
                        {
                            if (!int.TryParse(MatchValue, out matchInt))
                            {
                                errors.Add(new Errors.RequirementError { Message = "Key did not match expected type", SourceLine = base.LineContext });
                                validationResult = false;
                            }
                            matchObject = matchInt;
                        }
                        break;
                    case RegistryValueKind.QWord:
                        if (!string.IsNullOrEmpty(MatchValue))
                        {
                            if (!long.TryParse(MatchValue, out matchLong))
                            {
                                errors.Add(new Errors.RequirementError { Message = "Key did not match expected type", SourceLine = base.LineContext });
                                validationResult = false;
                            }
                            matchObject = matchLong;
                        }
                        break;
                    case RegistryValueKind.String:
                        matchObject = MatchValue;
                        break;
                    default:
                        matchObject = MatchValue;
                        break;
                }

                if (!string.IsNullOrEmpty(MatchValue))
                {
                    if (!string.IsNullOrEmpty(MinMatch))
                    {
                        if ((double)matchObject < minMatchDouble)
                        {
                            errors.Add(new Errors.RequirementError { Message = "Key did not exceed minimum value", SourceLine = base.LineContext });
                            validationResult = false;
                        }
                    }
                    if (!string.IsNullOrEmpty(MaxMatch))
                    {
                        if ((double)matchObject > maxMatchDouble)
                        {
                            errors.Add(new Errors.RequirementError { Message = "Key did exceeded maximum value", SourceLine = base.LineContext });
                            validationResult = false;
                        }
                    }
                }
                else if (validationResult && !PatternMatch && !string.IsNullOrEmpty(Comparitor))
                {
                    switch (Comparitor)
                    {
                        case "=":

                            if (matchObject == result)
                                validationResult = true;
                            errors.Add(new Errors.RequirementError { Message = "Key did not match", SourceLine = base.LineContext });
                            validationResult = false;
                            break;
                        case "<>":
                        case "!=":
                        case "not":
                            if (matchObject != result)
                                validationResult = true;
                            errors.Add(new Errors.RequirementError { Message = "Key did not match", SourceLine = base.LineContext });
                            validationResult = false;
                            break;
                        case ">":

                            if ((long)matchObject < (long)result)
                                return true;
                            errors.Add(new Errors.RequirementError { Message = "Key did not match", SourceLine = base.LineContext });
                            validationResult = false;
                            break;
                        case "<":
                            if ((long)matchObject > (long)result)
                                return true;
                            errors.Add(new Errors.RequirementError { Message = "Key did not match", SourceLine = base.LineContext });
                            validationResult = false;
                            break;
                    }
                }
                else if (PatternMatch)
                {
                    if (Regex.Match((string)result, MatchValue).Success)
                        validationResult = true;
                    else
                    {
                        errors.Add(new Errors.RequirementError { Message = "Key did not match", SourceLine = base.LineContext });
                        validationResult = false;
                    }
                }
            }

            return validationResult;
        }
        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "key")
                {
                    this.RegistryItem = new RegistryItem(reader.Value,this.XBDDef.Builder);
                }
                if (reader.Name == "subkey")
                {
                    this.SubKey = reader.Value;
                }
                if (reader.Name == "value")
                {
                    this.MatchValue = reader.Value;
                }
                if (reader.Name == "min-value")
                {
                    this.MinMatch = reader.Value;
                }
                if (reader.Name == "max-value")
                {
                    this.MaxMatch = reader.Value;
                }
                if (reader.Name == "pattern")
                {
                    this.MatchValue = reader.Value;
                    this.PatternMatch = true;
                }
            }
        }

        public override void Build(XBDElement parent)
        {
        }
    }
}
