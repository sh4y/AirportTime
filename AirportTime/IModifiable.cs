
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public interface IModifiable
{
    void AddModifier(string modifierName, double modifierValue);
    void RemoveModifier(string modifierName);
    double ApplyModifiers(double baseValue);
}
