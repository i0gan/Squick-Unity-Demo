
#pragma once
#include <iostream>
#include <squick/core/i_module.h>

class IPropertyModule
    : public IModule
{
public:
    enum PropertyGroup
    {
		NPG_JOB_LEVEL,
		NPG_FIGHTING_HERO,
		NPG_EFFECT_VALUE,
		NPG_REBIRTH_ADD,
		NPG_EQUIP,
		NPG_EQUIP_AWARD,
		NPG_STATIC_BUFF,
		NPG_RUNTIME_BUFF,
        NPG_ALL ,
    };

	//返回100以内的百分比数值(比如10，就应该是10%), 用来计算armor值代来代伤害减免，幸运值代来代幸运概率等
	static int Cal(int value)
	{
		float x = value / 10.0f;
		float f = 0.06f * x / (1.0f + (0.06f * x));
		return (int)(f * 100);
	}

	//高斯求和, 返回等级带来代数值结果,比如  + 10等级, 应该就是55,
	static int CalLevelValue(int level)
	{
		int result = 0;
		if (level > 0)
		{
			result = (1 + level) * level / 2;
		}

		return result;
	}

    virtual int64_t GetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType) = 0;
    virtual int SetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType, const int64_t nValue) = 0;

    virtual bool AddExp(const Guid& self, const int64_t exp) = 0;
    virtual bool FullHPMP(const Guid& self) = 0;
    virtual bool AddHP(const Guid& self, const int nValue) = 0;
    virtual bool ConsumeHP(const Guid& self, const int nValue) = 0;
    virtual bool EnoughHP(const Guid& self, const int nValue) = 0;
	virtual bool DamageHP(const Guid& self, const int nValue) = 0;

    virtual bool AddMP(const Guid& self, const int nValue) = 0;
    virtual bool ConsumeMP(const Guid& self, const int nValue) = 0;
    virtual bool EnoughMP(const Guid& self, const int nValue) = 0;
	virtual bool DamageMP(const Guid& self, const int nValue) = 0;

    virtual bool FullSP(const Guid& self) = 0;
    virtual bool AddSP(const Guid& self, const int nValue) = 0;
    virtual bool ConsumeSP(const Guid& self, const int nValue) = 0;
    virtual bool EnoughSP(const Guid& self, const int nValue) = 0;

    virtual bool AddGold(const Guid& self, const int64_t nValue) = 0;
    virtual bool ConsumeGold(const Guid& self, const int64_t nValue) = 0;
    virtual bool EnoughGold(const Guid& self, const int64_t nValue) = 0;

    virtual bool AddDiamond(const Guid& self, const int nValue) = 0;
    virtual bool ConsumeDiamond(const Guid& self, const int nValue) = 0;
	virtual bool EnoughDiamond(const Guid& self, const int nValue) = 0;

	virtual void ActiveExtraController() = 0;
};
