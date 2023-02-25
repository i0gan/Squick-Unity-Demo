#pragma once

#include <squick/plugin/kernel/i_kernel_module.h>
#include <squick/plugin/config/i_class_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/core/i_plugin_manager.h>
#include <squick/struct/protocol_define.h>
#include <squick/plugin/log/i_log_module.h>

#include "i_property_module.h"
#include "i_property_config_module.h"
#include <squick/core/base.h>

class PropertyModule : public IPropertyModule
{
public:
    PropertyModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual ~PropertyModule() {};

    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();

    virtual int64_t GetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType);
    virtual int SetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType, const int64_t nValue);

    virtual bool AddExp(const Guid& self, const int64_t exp);

    virtual bool FullHPMP(const Guid& self);
    virtual bool AddHP(const Guid& self, const int nValue);
    virtual bool ConsumeHP(const Guid& self, const int nValue);
	virtual bool EnoughHP(const Guid& self, const int nValue);
	virtual bool DamageHP(const Guid& self, const int nValue);

    virtual bool AddMP(const Guid& self, const int nValue);
    virtual bool ConsumeMP(const Guid& self, const int nValue);
	virtual bool EnoughMP(const Guid& self, const int nValue);
	virtual bool DamageMP(const Guid& self, const int nValue);

    virtual bool FullSP(const Guid& self);
    virtual bool AddSP(const Guid& self, const int nValue);
    virtual bool ConsumeSP(const Guid& self, const int nValue);
    virtual bool EnoughSP(const Guid& self, const int nValue);

    virtual bool AddGold(const Guid& self, const int64_t nValue);
    virtual bool ConsumeGold(const Guid& self, const int64_t nValue);
    virtual bool EnoughGold(const Guid& self, const int64_t nValue);

    virtual bool AddDiamond(const Guid& self, const int nValue);
    virtual bool ConsumeDiamond(const Guid& self, const int nValue);
    virtual bool EnoughDiamond(const Guid& self, const int nValue);

	virtual void ActiveExtraController();

protected:
    void RefreshBaseProperty(const Guid& self);

    void RefreshAllProperty(const Guid& self);

    int OnObjectClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var);

	int OnObjectMAXHPEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason);
    int OnObjectLevelEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason);
    int OnObjectConfigIDEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason);

    int OnRecordEvent(const Guid& self, const RECORD_EVENT_DATA& eventData, const SquickData& oldVar, const SquickData& newVar);

private:
	bool activeExtraController = false;

    IKernelModule* m_pKernelModule;
    IPropertyConfigModule* m_pPropertyConfigModule;
    IElementModule* m_pElementModule;
    IClassModule* m_pClassModule;
    ILogModule* m_pLogModule;
};
