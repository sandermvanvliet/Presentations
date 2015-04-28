# Continuous Delivery through the Cloud
Since I started working for Rabobank in the Market Risk team in 2013 we've been constantly working to improve the way we build and release our software to production while at the same time adopting Agile to streamline our development process. Over that time we've experimented a lot with the build and deployment process and have had many incarnations of the entire pipeline. In this blog post I'd like to walk you through the current version of our Continuous Delivery pipeline and why and how we arrived at this point.

# Continuous Delivery
But before we go into the details of our particular solution I'd like to revisit what Continuous Delivery means and why we chose to implement that at Market Risk.  

If you read about Continuous Delivery in books and on-line you'll find a definition that (usually) says that:
>Continuous Delivery is a software development practice where a team works to produce software that is of value to the business in short iterations and where the product can be reliably released at any time.

Wow.

Released at any time? Really?

Well yes, but this is a classical example where theory and practice don't necessarily meet. There are many different interpretations of what releasing actually means. For example it might mean that it can be released to an end-user for immediate use but it could also mean that it's just deployed to production but not used (or usable) yet. However the most common interpretation would be that the product itself is releasable at any time but the feature that is being worked doesn't have to be accessible yet.

Now releasing an application with features that are not complete yet may sound scary and you are right, it is, but that doesn't mean that it's bad. Imagine this: a team is working on feature X and happily coding away. Then in production a wild bug appears which needs a fix and it needs it yesterday. So what happens is that the team drops everything and works on fixing the bug but soon finds itself with the issue that the codebase has already changed! Half of feature X is in but it's not done yet...  

At this point you might find the team discussing all sorts of options:  

* Remove the changes on feature X and copy them back in later after the bug is fixed?
* Copy the source code of the production version and build and release that?
* Use that elaborate branching strategy we created when we started the project?

These options all feel suboptimal and bit risky to pull off. How would you know that a fix you apply in the production hot-fix branch is incorporated in the next feature release? If you pick a previous version can you be absolutely sure that no other bugs have been included in the meantime? And even if you have different branches, do you have a build & deployment pipeline for all of them?

To get around this problem the approach to making changes in the first place needs to be changed. Instead of working on a feature and committing when the work is done or that a feature is broken while it's being changed, developers need to ensure that whatever change they make doesn't break the application. Easier said than done, I can assure you.

But if you think about it, it actually might be simpler than it looks. Say that you're working on a new feature that involves a UI you could choose not to put the menu item in place up until the point that it's done. That way the code may be there, but it's still not accessible by the user. Only when you're done and comfortable that it works you can wire up your feature into a menu and make it available to the end user. A common technique for this is called *feature toggling*.

Now I'd happily discuss all the different tricks you can use to accomplish this but then this blog would never be finished so I'll save that for another time. But to wrap this up we can say that there are many options available that allow you to change the software and still be able to release to production.

Apart from making sure that a change to the software doesn't break the working of it is to ensure that the software does what it's supposed to. Because Continuous Delivery implies that you make changes, well, continuously, it would take too much time to manually retest the whole application to make sure that nothing broke.

A big part of implementing Continuous Delivery is to create automated test suites. These provide you with the confidence you need to be able to say: ***"Yes, this version can be released!"***. During the lifetime of the application a number of test suites are created. Typically you'll see a number of very small and fast running unit tests that validate small parts of the applications inner workings, integration tests that prove that these small parts collaborate together and larger acceptance tests that prove the business scenarios are implemented correctly.

These tests are run at different times in the development process. Unit tests are mostly used by the developers for that necessary quick feedback when they are changing the application. The integration and acceptance tests are typically used during the build process to check that everything works as expected. These tests also tend to run longer or require more set-up than the unit tests.

A typical build pipeline will look similar to this:

[Write test] -> [Write code] -> [Run unit tests] -> [Check-in] -> [Compile] -> [Run integration tests] -> [Run acceptance tests] -> [Done!]

Every step of this process provides feedback to the team whether or not the software is in a reliable state. Getting feedback early and often is one of the core concepts of Continuous Delivery and you should implement this in your process as early as you can. Tooling like TFS, Jenkins and TeamCity can help accomplishing this.

Once a team is at the stage where there are automated builds and tests there is one more thing that needs to be addressed. Being able to build software is nice but it still needs to be deployed to a production environment before it provides any value to the business. As with the builds and tests having an automated deployment is a must. I think that everyone can agree that manually deploying an application is stressful, tedious, error-prone and just plain boring at the same time. To ensure that you can deploy an application the same time over and over again it pays off to automate this process. There are many ways to do that, it can be a simple batch script or solutions like XL Deploy, Octopus or InRelease. The point is that being able to have a single set of deliverables that you can deploy to any environment gives you the confidence you need to deploy to production without making it an all-hands-on-deck experience.

So a long story short: Continuous Delivery is all about releasing to production whenever you want and with confidence supported by tooling.

# Our story so far
When we started building RAM+ a few years ago we wanted to get a build to our to-be-production environment as quick as possible. Our previous experiences with the nightly Toro builds led us in the direction of creating PowerShell scripts to perform the most common tasks from running the build and packaging the application to orchestrating the entire process.
This process was driven by a single scheduled task on our build server and would execute the following steps:

* Increase the version number for the build
* Queue a build in TFS and wait for the build to complete
* Generate the database scripts
* Package the application for all environments (there were about 7 of them)
* Deploy the application to our test environment
* Run the regression tests

In this process only the TFS build was using a standardized tool, the rest was a mix of PowerShell and batch files. While it might not be the cleanest solution it did work for us and we were in a situation where every night the software was built, deployed and tested automatically. You can imagine that this saves precious time because every morning that we came in we had our test results waiting for us.  

However we had a couple of issues we wanted to fix. For one the build process was hard to understand. Also it was hardly possible to run on a local machine, you always had to run an actual build which meant checking in your half-ready changes which might break the build and so on.

Another problem was that our deployment scripts had to be sophisticated enough to be able to deal with first uninstalling and then deploying again to the desired situation. There are quite a number of odd situations you can get yourself into.

Because the packages for all environments were generated at build time we found that our configuration became hard to manage as the application and configuration were tightly coupled. This means that whenever we wanted to create a new environment we had to make a change to our build process.

# Moving forward
One of the first steps we took was to separate the configuration from the code as much as we could. It turned out that while we developed RAM+ the settings ended up in various places, some might be in a file while others were in a database and yet others part of a deployment script. Utimately we settled on having as little in the configuration files and to keep most of our settings in the database, which makes them easily maintainable by our support group.

Moving the settings out of the package creates a new challenge: _Where to put the envrionment specific settings?_ Before we started using a deployment tool our scripts had a settings file per environment that contained all the necessary values to be able to deploy to said environment. This file together with the application package was all the deployment engineer needed to push a new version to any environment... ***NICE!*** Or maybe not?

Because of the security separation between the DBA and WinTel engineering groups our beautiful single integrated deployment script had to be broken up. The DBAs haven't got access to the Windows boxes and the WinTel engineer can't deploy the database. Unfortunately at the time we had no way to get around this, at least not in a procedural correct way.

Luckily there were other areas to improve our process.