
using CommandSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Ribbon.Abstractions;
using UnityCommander.Ribbon.Abstractions.Models;
using UnityCommander.Ribbon.Abstractions.Models.Controls;

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
                    var tab = model.Tabs
                        .FirstOrDefault(x => x.Id == tabDefinition.Id);

                    if (tab == null)
                    {
                        tab = new RibbonTabModel(
                            tabDefinition.Id,
                            tabDefinition.Title);

                        //if (tab.Groups.Count > 0)
                            model.Tabs.Add(tab);
                    }

                    foreach (var groupDefinition in tabDefinition.Groups)
                    {
                        var group = tab.Groups
                            .FirstOrDefault(x => x.Id == groupDefinition.Id);

                        if (group == null)
                        {
                            group = new RibbonGroupModel(
                                groupDefinition.Id,
                                groupDefinition.Title);

                                //if (group.Sections.Count > 0)
                                    tab.Groups.Add(group);
                        }

                        foreach (var sectionDefinition in groupDefinition.Sections)
                        {
                            var section = group.Sections
                                .FirstOrDefault(x => x.Id == sectionDefinition.Id);

                            if (section == null)
                            {
                                section = new RibbonGroupSectionModel(
                                    sectionDefinition.Id,
                                    sectionDefinition.Layout,
                                    new List<RibbonItemModel>());

                                //if (section.Items.Count > 0)
                                    group.Sections.Add(section);
                            }

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

                                        RibbonComboBoxDefinition comboBox =>
                                            CreateComboBox(comboBox),

                                        _ => null
                                    };

                                if (item != null)
                                    section.Items.Add(item);
                            }
                        }
                    }
                }
            }

            return model;
        }

        private RibbonItemModel CreateComboBox(
            RibbonComboBoxDefinition comboBox)
        {
            return new RibbonComboBoxModel
            {
                Id = comboBox.Id,
                IconKey = comboBox.IconKey,
                Items = comboBox.Items
                    .Select(x => new RibbonComboBoxItemModel
                    {
                        Id = x.Id,
                        IconKey = x.IconKey,
                        Title = x.Title,
                        CommandId = x.CommandId
                    })
                    .ToList()
            };
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
            var cmd = _commands.Get(button.CommandId);

            if (!_pluginCommands.TryGet(button.CommandId, out var def) && cmd == null)
                return null;

            return new RibbonButtonModel()
            {
                Id = button.CommandId,
                IconKey = button.IconKey,
                CommandId = def?.Id ?? cmd.Name,
            };
        }
    }
}
