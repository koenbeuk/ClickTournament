using Bunit;
using ScenarioTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClickTournament.Tests.Pages
{
    public partial class CounterScenarioTests
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public async Task PlayScenario(ScenarioContext scenario)
        {
            using var ctx = new TestContext();

            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime =
            app.Instance.GameTime = TimeSpan.FromSeconds(1);

            scenario.Fact("Starts in an initial state", () =>
            {
                Assert.Equal(App.GameState.Initial, app.Instance.State);
                Assert.Null(app.Instance.NextStateDateTime);
                Assert.Equal(0, app.Instance.Hits);
                Assert.Equal(0, app.Instance.Misses);
            });
            scenario.Fact("Scores are getting ignored when in started state", () =>
            {
                app.Instance.RecordHit();
                app.Instance.RecordMiss();

                // Assert
                Assert.Equal(0, app.Instance.Hits);
                Assert.Equal(0, app.Instance.Misses);
            });

            app.Instance.StartNewGame();
            
            scenario.Fact("Moved to a starting state", () =>
            {
                Assert.Equal(App.GameState.Starting, app.Instance.State);
                Assert.NotNull(app.Instance.NextStateDateTime);
            });
            scenario.Fact("Scores are getting ignored when in starting state", () =>
            {
                app.Instance.RecordHit();
                app.Instance.RecordMiss();

                Assert.Equal(0, app.Instance.Hits);
                Assert.Equal(0, app.Instance.Misses);
            });

            await Task.Delay(1100);

            scenario.Fact("Moved to a started state", () =>
            {
                Assert.Equal(App.GameState.Started, app.Instance.State);
                Assert.NotNull(app.Instance.NextStateDateTime);
            });
            scenario.Fact("Scores are getting recorded when in started state", () =>
            {
                app.Instance.RecordHit();
                app.Instance.RecordMiss();
                
                Assert.Equal(1, app.Instance.Hits);
                Assert.Equal(1, app.Instance.Misses);
            });

            await Task.Delay(2200);

            scenario.Fact("Moved to a finished state", () =>
            {
                Assert.Equal(App.GameState.Finished, app.Instance.State);
                Assert.Null(app.Instance.NextStateDateTime);
            });
            scenario.Fact("Scores are getting ignored when in finished state", () =>
            {
                app.Instance.RecordHit();
                app.Instance.RecordMiss();

                Assert.Equal(0, app.Instance.Hits);
                Assert.Equal(0, app.Instance.Misses);
            });
            scenario.Fact("Start second game moves state to starting game", () =>
            {
                app.Instance.StartNewGame();

                Assert.Equal(App.GameState.Starting, app.Instance.State);
                Assert.NotNull(app.Instance.NextStateDateTime);
            });
        }
    }
}
