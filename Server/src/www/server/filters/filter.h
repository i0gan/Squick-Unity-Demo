/**
 *
 *  filter.h
 *
 */

#pragma once

#include <drogon/HttpFilter.h>
using namespace drogon;


class filter : public HttpFilter<filter>
{
  public:
    filter() {}
    void doFilter(const HttpRequestPtr &req,
                  FilterCallback &&fcb,
                  FilterChainCallback &&fccb) override;
};

