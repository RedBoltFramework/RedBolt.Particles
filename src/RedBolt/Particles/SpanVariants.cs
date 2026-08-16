// Copyright (c) 2026 RedBolt and contributors.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

namespace RedBolt.Particles;

//template <typename T> SpanVariant<T>::SpanVariant(UpdateContext& context, ParticleGraphPinRef& pinRef)
//{
//    type_ = pinRef.type_;
//    if (type_ == ParticleGraphContainerType::Sparse)
//    {
//        data_ = context.layer_->GetAttributeValues<T>(pinRef.index_).data_;
//        indices_ = context.indices_.data();
//    }
//    else
//    {
//        const auto tempLocation = context.layer_->GetLayer()->GetIntermediateValues()[pinRef.index_];
//        data_ = reinterpret_cast<T*>(context.tempBuffer_.data() + tempLocation.offset_);
//        indices_ = nullptr;
//    }
//}
