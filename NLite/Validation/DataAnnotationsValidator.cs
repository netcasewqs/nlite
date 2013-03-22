using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using NLite.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using NLite.Internal;
using NLite.Collections;

namespace NLite.Validation.DataAnnotations
{
    class EntityValidator : IValidator
    {
        private static readonly Dictionary<Type, Type> customValidationMappings = new Dictionary<Type, Type>();

        private static readonly IDictionary<string, List<EntityMemberMapping>> EntityMemberMappingCache = new Dictionary<string, List<EntityMemberMapping>>();


        private static object Mutext = new object();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        public void Register(Type entityType, Type entityValidatorType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (entityValidatorType == null)
                throw new ArgumentNullException("entityValidatorType");

            lock (Mutext)
            {
                if (customValidationMappings.ContainsKey(entityType))
                    customValidationMappings.Remove(entityType);
                customValidationMappings.Add(entityType, entityValidatorType);
            }
        }

        public IErrorState Validate(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var type = instance.GetType();
            Type customValidatorType;
            if(customValidationMappings.TryGetValue(type,out customValidatorType))
                return Validate(instance, type, customValidatorType);

            var metadataTypeAttr = type.GetAttribute<System.ComponentModel.DataAnnotations.MetadataTypeAttribute>(false);
            if (metadataTypeAttr != null)
                return Validate(instance, type, metadataTypeAttr.MetadataClassType);

            var metadataTypeAttr2 = type.GetAttribute<NLite.Validation.EntityValidatorAttribute>(false);
            if (metadataTypeAttr2 != null)
                return Validate(instance, type, metadataTypeAttr2.EntityValidatorType);

            return Validate(instance, type);

        }

        private static IErrorState Validate(object instance, Type type)
        {
            List<EntityMemberMapping> memberMappings;
            var key = type.FullName ;
            if (!EntityMemberMappingCache.TryGetValue(key, out memberMappings))
            {
                memberMappings = (from prop in type.GetGetMembers()
                                  from attribute in prop.Member.GetAttributes<ValidationAttribute>(true)
                                  select new EntityMemberMapping(prop, attribute))
                                  .ToList();
                EntityMemberMappingCache.Add(key, memberMappings);
            }

            var state = new ErrorState();
            memberMappings.Where(m => !m.IsValid(instance))
                .ForEach(m => state.AddError(m.Name, m.ErrorMessage()));

            return state;
        }

        private static IErrorState Validate(object instance, Type type, Type metadataType)
        {
            List<EntityMemberMapping> memberMappings;

            var key = type.FullName + "|" + metadataType.FullName;
            if (!EntityMemberMappingCache.TryGetValue(key, out memberMappings))
            {
                var items = from m in type.GetGetMembers()
                            let p = metadataType.GetMember(m.Name).FirstOrDefault()
                            where p != null
                            select new { MetadataMember = p, MemberModel = m };
                memberMappings =( from m in items
                                 from attribute in m.MetadataMember.GetAttributes<ValidationAttribute>(true)
                                 select new EntityMemberMapping(m.MemberModel, attribute))
                                 .ToList();
                EntityMemberMappingCache.Add(key, memberMappings);

            }

            var state = new ErrorState();
            memberMappings.Where(m => !m.IsValid(instance))
                .ForEach(m => state.AddError(m.Name, m.ErrorMessage()));

            return state;
        }

    }

    class EntityMemberMapping
    {
        private MemberInfo Member;
        private Getter Getter;
        private ValidationAttribute ValidationAttribute;
        private readonly  string DisplayName;
        public readonly string Name;
        public EntityMemberMapping(MemberModel memberModel, ValidationAttribute attr)
        {
            Member = memberModel.Member;
            Name = memberModel.Name;
            Getter = memberModel.GetMember;
            ValidationAttribute = attr;
            var displayNameAttr = Member.GetAttribute<DisplayNameAttribute>(false);
            DisplayName = displayNameAttr != null ? displayNameAttr.DisplayName : Member.Name;
        }

        public bool IsValid(object instance)
        {
            return ValidationAttribute.IsValid(Getter(instance));
        }

        public string ErrorMessage()
        {
            return ValidationAttribute.FormatErrorMessage(DisplayName);
        }
    }
}
