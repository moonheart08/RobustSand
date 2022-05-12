# How do I add a new particle type (element)?
- Add your new type of particle to [the ParticleType enum](https://github.com/moonheart08/RobustSand/blob/master/Content.Client/Simulation/ParticleKinds/Abstract/ParticleType.cs)
- Create a new class in one of the folders in Content.Client/Simulation/ParticleKinds (if you can't figure out where it goes, try and think of a new category to put it in.)
- Inherit ParticleImplementation with the class, and implement the required fields:
    - PType should match the ParticleType value you added.
    - PName should be a (short) name of the particle.
    - PDescription should be a description of the particle. Jokes are ok if it's nothing complex.
    - PWeight dictates what particles it will try and pass through and what it will float atop.
      
      Look at other particles you think it shouldn't pass through for weight values. Solids should have weight 255.
    - PColor is the color of your particle. Try and pick one that isn't too close to another particle of the same type (i.e. two solids with the same color and render type is bad.)
    - PMovementFlags dictates how your particle can move. Look at the documentation for ParticleMovementFlag for info.
    - PPropertyFlags controls particle properties, like if it should be treated as a solid by other particles, and if it can be melted by acid.
- Optionally, override ParticleImplementation.Update() to give your particle custom update code to run before movement.
    - Particle setup should be done in Spawn() and teardown in Delete(). There's a whole bunch of other behavior override methods, check the xmldocs.
- Add the [Particle] attribute to your new class.
- Presto, you've added a particle type.