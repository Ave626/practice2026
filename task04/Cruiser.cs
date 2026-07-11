namespace task04;

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;
    public int Position { get; private set; }
    public int Angle { get; private set; }
    public int Ammo { get; private set; } = 50; 

    public void MoveForward(){
        Position += Speed; 
    }

    public void Rotate(int angle){
        Angle += angle;
    }
    
    public void Fire(){
        if (Ammo > 0){
            Ammo--; 
        }
    }
}
