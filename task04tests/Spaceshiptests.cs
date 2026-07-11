using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_MoveForward_ShouldIncreasePosition(){
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        
        Assert.Equal(50, cruiser.Position); 
    }

    [Fact]
    public void Fighter_Rotate_ShouldChangeAngle(){
        var fighter = new Fighter();
        fighter.Rotate(90);
        
        Assert.Equal(90, fighter.Angle);
    }

    [Fact]
    public void Spaceship_Fire_ShouldDecreaseAmmo(){
        var fighter = new Fighter();
        int initialAmmo = fighter.Ammo;
        
        fighter.Fire();
        Assert.Equal(initialAmmo - 1, fighter.Ammo);
    }
}
