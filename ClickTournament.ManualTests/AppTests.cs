using Bunit;
using ScenarioTests;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace ClickTournament.ManualTests
{
    public partial class AppTests
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void BasicScenario(ScenarioContext scenario)
        {
            using var context = new TestContext();

            var app = context.RenderComponent<App>();
            app.Instance.WarmupTime = TimeSpan.FromSeconds(1);
            app.Instance.GameTime = TimeSpan.FromSeconds(1);

            scenario.Fact("Initial state", () =>
                Assert.Equal(App.GameState.Initial, app.Instance.State));

            app.Instance.StartNewGame();

            scenario.Fact("Starting state", () =>
                Assert.Equal(App.GameState.Starting, app.Instance.State));

            app.Instance.RecordHit();
            app.Instance.RecordMiss();
            
            scenario.Fact("Hits and misses are ignored in the starting state", () => {
                Assert.Equal(0, app.Instance.Hits);
                Assert.Equal(0, app.Instance.Misses);
            });

            Thread.Sleep(app.Instance.WarmupTime.Add(TimeSpan.FromSeconds(0.5)));

            scenario.Fact("Started state", () =>
            {
                Assert.Equal(App.GameState.Started, app.Instance.State);
            });

            app.Instance.RecordHit();
            app.Instance.RecordMiss();

            Thread.Sleep(app.Instance.GameTime.Add(TimeSpan.FromSeconds(0.5)));

            scenario.Fact("Finished state", () =>
            {
                Assert.Equal(App.GameState.Finished, app.Instance.State);
            });

            scenario.Fact("Recorded hits", () =>
                Assert.Equal(1, app.Instance.Hits));

            scenario.Fact("Record misses", () =>
                Assert.Equal(1, app.Instance.Misses));

            scenario.Fact("Hits are resetted to 0 when starting a new game", () =>
            {
                app.Instance.StartNewGame();
                Assert.Equal(0, app.Instance.Hits);
            });

            scenario.Fact("Misses are resetted to 0 when starting a new game", () =>
            {
                Debug.Assert(1 == app.Instance.Misses);

                app.Instance.StartNewGame();
                Assert.Equal(0, app.Instance.Misses);
            });
        }
    }
}
