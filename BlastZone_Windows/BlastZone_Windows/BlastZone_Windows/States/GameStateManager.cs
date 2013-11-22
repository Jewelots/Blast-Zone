using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BlastZone_Windows.States
{
    public enum StateType
    {
        MENU,
        LOBBY,
        CONTROLS,
        OPTIONS,
        WINSCREEN,
        TIESCREEN,
        GAME,
        NONE
    }

    class GameStateManager
    {
        class StateEventArgs : EventArgs
        {
            public StateType state;

            public StateEventArgs(StateType s)
            {
                state = s;
            }
        }

        Dictionary<StateType, GameState> gameStates;

        GameState currentState;

        Drawing.ScreenTransitionInOut screenTransition;

        MainGame mainGame;

        bool transitioning = false;
        bool transitionMusic = false;

        StateType toTransitionNext;
        StateType stateTransitioningTo;

        public GameStateManager(MainGame mainGame, GraphicsDevice graphicsDevice)
        {
            gameStates = new Dictionary<StateType, GameState>();
            gameStates[StateType.MENU] = new MenuState(this);
            gameStates[StateType.LOBBY] = new LobbyState(this);
            gameStates[StateType.CONTROLS] = new ControlsState(this);
            gameStates[StateType.OPTIONS] = new OptionsState(this);
            gameStates[StateType.WINSCREEN] = new WinScreenState(this);
            gameStates[StateType.TIESCREEN] = new TieScreenState(this);
            gameStates[StateType.GAME] = new GameplayState(this, graphicsDevice);

            currentState = gameStates[StateType.MENU];

            screenTransition = new Drawing.ScreenTransitionInOut();
            toTransitionNext = StateType.NONE;
            stateTransitioningTo = StateType.NONE;

            this.mainGame = mainGame;
        }

        public void LoadContent(ContentManager Content)
        {
            foreach (KeyValuePair<StateType, GameState> statePair in gameStates)
            {
                statePair.Value.LoadContent(Content);
            }

            screenTransition.LoadContent(Content);

            //Only enter after loaded content to be sure
            currentState.Enter();
        }

        public void SwapState(StateType state)
        {
            //Don't transition to nothing
            if (state == StateType.NONE) return;

            if (currentState != null)
            {
                currentState.Exit();
            }

            Managers.ParticleManager.ClearAll();

            currentState = gameStates[state];

            currentState.Enter();
        }

        void SwapState(EventArgs e)
        {
            StateEventArgs stateEventArgs = e as StateEventArgs;

            SwapState(stateEventArgs.state);
        }

        void StopTransition(EventArgs e)
        {
            stateTransitioningTo = StateType.NONE;
            transitioning = false;
            screenTransition.Reset();
        }

        public void SwapStateWithTransition(StateType state, bool transitionMusic)
        {
            //Don't transition to nothing
            if (state == StateType.NONE) return;

            this.transitionMusic = transitionMusic;

            if (!transitioning)
            {
                screenTransition.Reset();

                screenTransition.OnTransition += SwapState;
                screenTransition.OnTransitionFinished += StopTransition;
                screenTransition.SetEventArgs(new StateEventArgs(state));

                transitioning = true;
                stateTransitioningTo = state;
            }
            else
            {
                //Don't transition to something already transitioning to
                if (stateTransitioningTo == state) return;

                toTransitionNext = state;
            }
        }

        public void SwapStateWithTransition(StateType state)
        {
            SwapStateWithTransition(state, false);
        }

        public void SwapStateWithTransitionMusic(StateType state)
        {
            SwapStateWithTransition(state, true);
        }

        public void Update(GameTime gameTime)
        {
            if (currentState == null) return;

            currentState.Update(gameTime);

            if (transitioning)
            {
                screenTransition.Update(gameTime);

                if (transitionMusic)
                {
                    MediaPlayer.Volume = screenTransition.FadeAmount() * GlobalGameData.MusicVolume;
                }
            }
            else
            {
                if (toTransitionNext != StateType.NONE)
                {
                    SwapStateWithTransition(toTransitionNext, transitionMusic);
                    toTransitionNext = StateType.NONE;
                }
            }
        }

        public GameState GetState(StateType state)
        {
            return gameStates[state];
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (currentState == null) return;

            currentState.Draw(spriteBatch, gameTime);

            spriteBatch.Begin();

            if (transitioning)
            {
                screenTransition.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void QuitGame()
        {
            mainGame.Exit();
        }

        public bool IsTransitioning()
        {
            return transitioning;
        }
    }
}
