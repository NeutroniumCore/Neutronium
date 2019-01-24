# Neutronium Building


<p align="center"><img <p align="center"><img width="200"src="../images/logo/bb-logo.png"></p>

## Overview


Neutronium BuildingBlocks is a separated project that provides opinionated solutions to build [Neutronium](https://github.com/NeutroniumCore/Neutronium) application:

Is has its own [github repo](https://github.com/NeutroniumCore/Neutronium.BuildingBlocks/blob/master/README.md) and its own [nuget packages](https://www.nuget.org/packages/Neutronium.BuildingBlocks.Standard/).


## Assemblies

### ApplicationTools
Provides interfaces for common application features such as native message box, native file and directory picker...

### Wpf
Provides an implementation for `ApplicationTools` interfaces based on Wpf framework.

### Application
Provides solution for application architecture including:
  - routing (integrated with vue via [vue-cli-plugin-neutronium](https://github.com/NeutroniumCore/vue-cli-plugin-neutronium)).
  - Dependency injection for main View-models
  - API for modal and notifications
  
### SetUp
Aims at making it easy to switch between different debug modes and make the usage of `live reload` easy. It provides utility to run npm scripts and to manage application mode.


See [complete documentation](https://neutroniumcore.github.io/Neutronium.BuildingBlocks/) for more information.
