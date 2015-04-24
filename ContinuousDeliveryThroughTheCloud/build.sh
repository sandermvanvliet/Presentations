#!/bin/sh

if [ -d "build" ]
then
  rm -f build/*
fi

if [ ! -d "build" ]
then
  mkdir build
fi

node_modules/marked/bin/marked --gfm --smartlists --smartypants "Continuous Delivery Through The Cloud.md" -o "build/markdown.html"

cat pre.html build/markdown.html post.html > build/index.html
rm build/markdown.html
cp github.css build
