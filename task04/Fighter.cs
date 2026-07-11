namespace task04;

public class Fighter : ISpaceship{
    public int Speed => 100;
    public int FirePower => 50;

    public int Position {get; private set;}
    public int Angle {get; private set;}
    public int Ammo {get; private set;} = 200;

    public void MoveForward(){
        Position += Speed;
    }

    public void Rotate(int angle){
        Angle += angle;
    }

    public void Fire(){
        if(Ammo > 0){
            Ammo--;
        }
    }

}
