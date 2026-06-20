
using CommandSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Ribbon.Abstractions;
using UnityCommander.Ribbon.Abstractions.Models;
using UnityCommander.Ribbon.Abstractions.Models.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RibbonModelFactory : IRibbonModelFactory
    {
        private readonly ICommandRegistry _commands;
        private readonly IPluginCommandRegistry _pluginCommands;
        private readonly IRibbonRegistry _ribbonRegistry;

        public RibbonModelFactory(
            ICommandRegistry commands,
            IPluginCommandRegistry pluginCommands,
            IRibbonRegistry ribbonRegistry)
        {
            _commands = commands;
            _ribbonRegistry = ribbonRegistry;
            _pluginCommands = pluginCommands;
        }

        public RibbonModel Create()
        {
            var model = new RibbonModel();

            foreach (var contribution in _ribbonRegistry.Contributions)
            {
                foreach (var tabDefinition in contribution.Definition.Tabs)
                {
                    var tab = new RibbonTabModel(
                        tabDefinition.Id,
                        tabDefinition.Title);

                    foreach (var groupDefinition in tabDefinition.Groups)
                    {
                        var group = new RibbonGroupModel(
                            groupDefinition.Id,
                            groupDefinition.Title);

                        foreach (var sectionDefinition in groupDefinition.Sections)
                        {
                            var section = new RibbonGroupSectionModel(
                                sectionDefinition.Id,
                                sectionDefinition.Layout,
                                new List<RibbonItemModel>());

                            foreach (var itemDefinition in sectionDefinition.Items)
                            {
                                RibbonItemModel? item =
                                    itemDefinition switch
                                    {
                                        RibbonButtonDefinition button =>
                                            CreateButton(button),

                                        RibbonCheckBoxDefinition checkBox =>
                                            CreateCheckBox(checkBox),

                                        RibbonRadioButtonDefinition radio =>
                                            CreateRadioButton(radio),

                                        _ => null
                                    };

                                if (item != null)
                                    section.Items.Add(item);
                            }

                            if (section.Items.Count > 0)
                                group.Sections.Add(section);
                        }

                        if (group.Sections.Count > 0)
                            tab.Groups.Add(group);
                    }

                    if (tab.Groups.Count > 0)
                        model.Tabs.Add(tab);
                }
            }

            return model;
        }

        private RibbonItemModel CreateCheckBox(RibbonCheckBoxDefinition checkBox)
        {
            return null;
        }

        private RibbonItemModel CreateRadioButton(RibbonRadioButtonDefinition radio)
        {
            return null;
        }

        private RibbonItemModel CreateButton(RibbonButtonDefinition button)
        {
            if (!_pluginCommands.TryGet(button.CommandId, out var def))
                return null;

            return new RibbonButtonModel()
            {
                Id = button.CommandId,
                IconKey = button.IconKey,
                CommandId = def.Id,
            };
        }
    }
}
