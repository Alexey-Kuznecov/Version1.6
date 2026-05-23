using System;
using UnityCommander.Modules.FilePanel.Docking.Diff;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;
using Xunit;

namespace UnityCommander.DockingSync.Tests
{
    public class DiffEngineTests
    {
        [Fact]
        public void Diff_ShouldDetect_AddedTab()
        {
            var panelId = Guid.NewGuid();
            var tab1 = Guid.NewGuid();
            var tab2 = Guid.NewGuid();

            var oldSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new() { tab1 }
                    }
                }
            };

            var newSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new() { tab1, tab2 }
                    }
                }
            };

            var engine = new DockingDiffEngine();

            var result = engine.Diff(oldSnap, newSnap);

            Assert.Single(result.AddedTabs);
            Assert.Equal(tab2, result.AddedTabs[0]);
        }

        [Fact]
        public void Diff_ShouldDetect_RemovedTab()
        {
            var panelId = Guid.NewGuid();
            var tab = Guid.NewGuid();

            var oldSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new() { tab }
                    }
                }
            };

            var newSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new()
                    }
                }
            };

            var engine = new DockingDiffEngine();

            var result = engine.Diff(oldSnap, newSnap);

            Assert.Single(result.RemovedTabs);
            Assert.Equal(tab, result.RemovedTabs[0]);
        }

        [Fact]
        public void Diff_ShouldDetect_MovedTab()
        {
            var panel1 = Guid.NewGuid();
            var panel2 = Guid.NewGuid();
            var tab = Guid.NewGuid();

            var oldSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panel1,
                        Tabs = new() { tab }
                    }
                }
            };

            var newSnap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panel2,
                        Tabs = new() { tab }
                    }
                }
            };

            var engine = new DockingDiffEngine();

            var result = engine.Diff(oldSnap, newSnap);

            Assert.Single(result.MovedTabs);

            var move = result.MovedTabs[0];

            Assert.Equal(tab, move.TabId);
            Assert.Equal(panel1, move.FromPanel);
            Assert.Equal(panel2, move.ToPanel);
        }

        [Fact]
        public void Diff_ShouldReturnEmpty_WhenNoChanges()
        {
            var panel = Guid.NewGuid();
            var tab = Guid.NewGuid();

            var snap = new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panel,
                        Tabs = new() { tab }
                    }
                }
            };

            var engine = new DockingDiffEngine();

            var result = engine.Diff(snap, snap);

            Assert.Empty(result.AddedTabs);
            Assert.Empty(result.RemovedTabs);
            Assert.Empty(result.MovedTabs);
        }
    }
}