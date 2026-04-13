using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Engine;

// ------------------------------------------------------- 
// To create a new game inherit this class, 
// attach a render window, a player and the first level  
//
// player 
// window
// level 
// ------------------------------------------------------- 

public class Game {
    
   // fixed size grid
   public const int width  = 78;
   public const int height = 25;

   protected Scene?         _currentLevel;
   protected bool           _isQuit;
   protected IRenderWindow? _window;
   protected Player?        _player;
   public IRenderWindow Window => _window;
    public Game() {
      _isQuit = false;
   }

   public void run() {
      // the game loop
      while (_currentLevel!.IsActive) {
         // ---------------
         // draw the level 
         // ---------------
         if (_window is null)
            throw new Exception("Game window not initialized");

         _currentLevel!.Draw(_window);
         _window!.Display();

         // -----------------
         // handle user input 
         // -----------------
         HandleUserInput();

         // -----------------
         // update the level
         // ----------------- 
         _currentLevel!.Update();
      }
   }


   protected virtual void HandleUserInput() {
        if (_currentLevel == null)
            return;
        ConsoleKeyInfo key = Console.ReadKey(true);
      if (_currentLevel!.HasCommand(key.Key))
         _currentLevel!.DoCommand(new Command(_currentLevel!.GetCommand(key.Key)));
   }
   public virtual void SaveToFile(string path) { }


}