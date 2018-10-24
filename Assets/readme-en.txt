/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_
// CurlNoise Particle System
// c 2018 Kazuya Hiruma
// Version 1.0.0
/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_

CurlNoise Particle System is GPU based using Curl Noise particle system.
You can control behaviour with CurlNoise's parameters.

=====================================================================================
## Update history


Version 1.0.0:
- First Release
=====================================================================================


## How to use

### Setup

You should put a "CurlParticleSystem" prefab in your scene.
"CurlParticleSystem" provide a object pooling and "CurlParticle" instances.
You can set limit object pooling in the inspector at "Limit" property.


### Profile object

You can setup a particle behaviour by "CurlParticleProfile" (It's a scriptable object).

The parameters are controlling particle's behaviour.
Turn on the "Show Gizmos" property of "CurlParticle" component then you can see a visualized parameters.


#### "Normal" and "Fake" type meaning

You can choise a type on a profile object.
The normal type is correctory caluculation of Curl Noise.

Other hand, the fake type is incorrectory caluculation of Curl Noise, but that provide fun animation.

The asset include both profiles, "Normal" and "Fake", you can create a profile based these profiles.


### Emitter

The asset include some Emitters.


#### CurlParticleEmitter

This is just normal emitter. The emitter use a CurlParticle object as normal emit.


#### ShapeEmitter

This is emit particles on a mesh surface.


#### DistanceEmitter

This is emit particles by moving distance.

