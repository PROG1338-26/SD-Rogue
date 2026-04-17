using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

namespace RlGameNS;


public class MyGame : Game {
   
   private void init() {
      // To create a new game just need to 
      // 'inject' an IRenderWindow to draw the game one
      // 'inject' a Player, the player lives outside or the Scene's because the 
      // player visits all the scenes and takes their inventory with them. 
      // you must load the first leveel, and your level or your game must manage 
      // the level switching. 
      
      _window       = new ScreenBuff();
      _player       = new Rogue();
      _currentLevel = new Level(_player, map1, this);
      
   }

   public MyGame() {
      // init level on construction 
      init();
   }


    // ----------------------------------------------------------------
    // string to use as the backgound on our first level
    // Added 3 blank lines at the top, removed 3 from the bottom.
    // This shifts the entire dungeon down to make room for the HUD.
    // Compressed the map by 2 rows internally so that the 4 message lines 
    // at the top and the 1 HUD line at the bottom fit perfectly within Game.height (25).
    // ----------------------------------------------------------------
    // ----------------------------------------------------------------

    public const string map1 =
      """
      
      
      
      
               ┌──────┐          ┌─────────────┐            ┌───────┐
               │......│        ##+.............+############+.......│
               │......+######### └──────────+──┘            │.......│
               │......│                     #               └───────┘
               └──+───┘                     #
           ########                 #########
      ┌────+┐                     ┌─+───────┐              ┌──────────────────┐
      │.....│                     │.........│              │..................│
      │.....+#####################+.........│              │..................│
      │.....│                     │.........│              │..............>...│
      │.....│                     │.........+##############+..................│
      └─+───┘                     └───+─────┘              └───────────────+──┘
        #                             #                                    #
        ######               ┌────────+──────────────┐                     #
             #             ##+.......................|                     #
             #             # |.......................|   ###################
             #             # |.......................|   #
             #             # |.......................+####
             #             # └───────────────────────┘
             ###############
      """;
}