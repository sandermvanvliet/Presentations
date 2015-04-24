# Continuous Delivery through the Cloud
Since I started working for Rabobank in the Market Risk team in 2013 we've been constantly working to improve the way we build and release our software to production while at the same time adopting Agile to streamline our development process. Over that time we've experimented a lot with the build and deployment process and have had many incarnations of the entire pipeline. In this blog post I'd like to walk you through the current version of our Continuous Delivery pipeline and why and how we arrived at this point.

# Continuous Delivery
But before we go into the details of our particular solution I'd like to revisit what Continuous Delivery means and why we chose to implement that at Market Risk.  

If you read about Continuous Delivery in books and on-line you'll find a definition that (usually) says that:
>Continuous Delivery is a software development practice where a team works to produce software that is of value to the business in short iterations and where the product can be reliably released at any time.

Wow.

Released at any time? Really?

Well yes, but this is a classical example where theory and practice don't necessarily meet. There are many different interpretations of what releasing actually means. For example it might mean that it can be released to an end-user for immediate use but it could also mean that it's just deployed to production but not used (or usable) yet. However the most common interpretation whould be that the product itself is releasable at any time but the feature that is being worked doesn't have to be accessible yet.

Now releasing an application with features that are not complete yet may sound scary and you are right, it is, but that doesn't mean that it's bad. Imagine this: a team is working on feature X and happily coding away. Then in production a wild bug appears which needs a fix and it needs it yesterday. So what happens is that the team drops everything and works on fixing the bug but soon finds itself with the issue that the codebase has already changed! Half of feature X is in but it's not done yet...  

At this point you might find the team discussing all sorts of options:  

* Remove the changes on feature X and copy them back in later after the bug is fixed?
* Copy the source code of the production version and build and release that?
* Use that elaborate branching strategy we created when we started the project?

These options all feel suboptimal and bit risky to pull off. How would you know that a fix you apply in the production hotfix branch is incorporated in the next feature release? If you pick a previous version can you be absolutely sure that no other bugs have been included in the meantime? And even if you have different branches, do you have a build & deployment pipeline for all of them?

To get around this problem the approach to making changes in the first place needs to be changed. Instead of working on a feature and committing when the work is done or that a feature is broken while it's being changed, developers need to ensure that whatever change they make doesn't break the application. Easier said than done, I can assure you.

But if you think about it, it actually might be simpler than it looks. Say that you're working on a new feature that involves a UI you could choose not to put the menu item in place up until the point that it's done. That way the code may be there, but it's still not accessible by the user. Only when you're done and comfortable that it works you can wire up your feature into a menu and make it available to the end user. A common technique for this is called *feature toggling*.

Now I'd happily discuss all the different tricks you can use to accomplish this but then this blog would never be finished so I'll save that for another time. But to wrap this up we can say that there are many options available that allow you to change the software and still be able to release to production.