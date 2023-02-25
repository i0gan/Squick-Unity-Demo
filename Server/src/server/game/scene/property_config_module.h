
#pragma once

#include "third_party/rapidxml/rapidxml.hpp"
#include "third_party/rapidxml/rapidxml_print.hpp"
#include "third_party/rapidxml/rapidxml_utils.hpp"
#include <squick/plugin/config/i_class_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/struct/protocol_define.h>

#include "i_property_config_module.h"
class PropertyConfigModule
    : public IPropertyConfigModule
{
public:
    PropertyConfigModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual ~PropertyConfigModule() {};

    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();

    virtual bool LegalLevel(const int nJob, const int nLevel);

    virtual const std::string& GetInitPropertyID(const int nJob,  const int nLevel);

    virtual void ClearInitPropertyData();

    virtual void AddInitPropertyID(const int nJob, const int nLevel, const std::string& data);

    virtual void SetEx(const bool b);

protected:
    void Load();

    std::map<Guid, std::string>& GetData();

private:
    bool mbExtra = false;

	//[job,Level]->ConfigID
    std::map<Guid, std::string> mhtCoefficientData;

    IClassModule* m_pClassModule;
    IElementModule* m_pElementModule;
};
