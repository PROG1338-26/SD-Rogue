using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Interfaces;

internal interface IDamageable
{
   int Attack();

   void TakeDamage(int damage);
}
