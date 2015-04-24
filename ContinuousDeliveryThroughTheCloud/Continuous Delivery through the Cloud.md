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
* Use an elaborate branching strategy?

