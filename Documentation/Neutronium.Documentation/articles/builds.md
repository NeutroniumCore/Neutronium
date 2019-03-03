<a href="https://github.com/NeutroniumCore/Neutronium" target="_blank">
  <img
    style="position: fixed; top: 0; right: 0; border: 0; z-index:99999"
    width="149"
    height="149"
    src="https://github.blog/wp-content/uploads/2008/12/forkme_right_gray_6d6d6d.png?resize=149%2C149"
    class="attachment-full size-full"
    alt="Fork me on GitHub"
    data-recalc-dims="1"
  />
</a>


# Builds

## 32 and 64 bits builds are supported
   Since version 1.2.0. it is possible to use 64 build version of ChromiumFx.<br>
    If you build a Neutronium project for x64 platform, the 64 bits build version of CEF will be used.<br>
  AnyCPU builds use 64 bits build version of CEF.
   
## Checking version using about Window:

Check `platform` information in the about window:

![debug buttons](../images/tools/about-64-bits.png)


## Gotchas:
- Make sure that all the projects of the solution have the same platform value. Supported values: `x86`, `x64` or `Any CPU`.
- Make sure that the flag `Prefer 32-bit` of the project (Properties>Build) is set to false.