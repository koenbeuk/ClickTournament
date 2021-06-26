using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClickTournament.Tests.Pages
{
    public class NonScenarioTests
    {
        [Fact]
        public void StartsInAnInitialState()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();

            // Assert
            Assert.Equal(App.GameState.Initial, app.Instance.State);
            Assert.Null(app.Instance.NextStateDateTime);
            Assert.Equal(0, app.Instance.Hits);
            Assert.Equal(0, app.Instance.Misses);
        }

        [Fact]
        public void StartNewGame_MovesStateToStarting()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.StartNewGame();

            // Assert
            Assert.Equal(App.GameState.Starting, app.Instance.State);
            Assert.NotNull(app.Instance.NextStateDateTime);
        }

        [Fact]
        public async Task StartNewGame_AfterStartingPeriod_MovesStateToStarted()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(1100);

            // Assert
            Assert.Equal(App.GameState.Started, app.Instance.State);
            Assert.NotNull(app.Instance.NextStateDateTime);
        }

        [Fact]
        public async Task StartNewGame_AfterGameEnded_MovesStateToFinished()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime =
            app.Instance.GameTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(2200);

            // Assert
            Assert.Equal(App.GameState.Finished, app.Instance.State);
            Assert.Null(app.Instance.NextStateDateTime);
        }

        [Fact]
        public async Task StartSecondGame_MovesStateToStarting()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime =
            app.Instance.GameTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(3000);

            app.Instance.StartNewGame();

            // Assert
            Assert.Equal(App.GameState.Starting, app.Instance.State);
            Assert.NotNull(app.Instance.NextStateDateTime);
        }

        [Fact]
        public void ScoresAreGettingIgnoredWhenInInitialState()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.RecordHit();
            app.Instance.RecordMiss();

            // Assert
            Assert.Equal(0, app.Instance.Hits);
            Assert.Equal(0, app.Instance.Misses);
        }

        [Fact]
        public void ScoresAreGettingIgnoredWhenInStartingState()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.StartNewGame();
            app.Instance.RecordHit();
            app.Instance.RecordMiss();

            // Assert
            Assert.Equal(0, app.Instance.Hits);
            Assert.Equal(0, app.Instance.Misses);
        }

        [Fact]
        public async Task ScoresAreGettingRecordedWhenInStartedState()
        {
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(1100);
            app.Instance.RecordHit();
            app.Instance.RecordMiss();

            // Assert
            Assert.Equal(1, app.Instance.Hits);
            Assert.Equal(1, app.Instance.Misses);
        }

        [Fact]
        public async Task ScoresAreGettingIgnoredWhenInFinishedState()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime =
            app.Instance.GameTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(2200);

            app.Instance.RecordHit();
            app.Instance.RecordMiss();

            // Assert
            Assert.Equal(0, app.Instance.Hits);
            Assert.Equal(0, app.Instance.Misses);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task WeCanHitNumerousTimes(int hitSample)
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(1100);

            for (var i = 0; i < hitSample; i++)
            {
                app.Instance.RecordHit();
            }

            // Assert
            Assert.Equal(hitSample, app.Instance.Hits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task WeCanMissNumerousTimes(int missSample)
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var app = ctx.RenderComponent<App>();
            app.Instance.WarmupTime = TimeSpan.FromSeconds(1);
            app.Instance.StartNewGame();

            await Task.Delay(1100);

            for (var i = 0; i < missSample; i++)
            {
                app.Instance.RecordHit();
            }

            // Assert
            Assert.Equal(missSample, app.Instance.Hits);
        }
    }
}
